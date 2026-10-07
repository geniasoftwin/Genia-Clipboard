using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GeniaClipboard;

internal static class EncryptedHistoryCodec
{
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("GCVLT001");
    private static readonly byte[] AadPrefix = Encoding.ASCII.GetBytes("GeniaClipboard.History.v1");
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int PortableSaltSize = 16;
    private const int KeySize = 32;
    private const int Pbkdf2Iterations = 600_000;
    internal const long MaxVaultFileSize = 192L * 1024 * 1024;

    internal sealed record Header(VaultMode Mode, byte[] Salt, byte[] Nonce, byte[] Tag, int CiphertextLength);

    public static VaultMode ProbeMode(string path)
    {
        using var stream = OpenCheckedRead(path);
        Span<byte> header = stackalloc byte[Magic.Length + 1];
        ReadExactly(stream, header);
        ValidateMagic(header[..Magic.Length]);

        var mode = (VaultMode)header[^1];
        ValidateMode(mode);
        return mode;
    }

    public static byte[] ReadSalt(string path)
    {
        using var stream = OpenCheckedRead(path);
        ReadAndValidateMagic(stream);
        var modeValue = stream.ReadByte();
        if (modeValue < 0)
        {
            throw new InvalidDataException("Повреждён заголовок зашифрованной истории.");
        }

        var mode = (VaultMode)modeValue;
        ValidateMode(mode);

        var saltLength = stream.ReadByte();
        if (saltLength < 0 || saltLength > 64)
        {
            throw new InvalidDataException("Некорректная соль зашифрованной истории.");
        }

        var salt = new byte[saltLength];
        ReadExactly(stream, salt);

        if (mode == VaultMode.Portable && salt.Length != PortableSaltSize)
        {
            throw new InvalidDataException("Некорректная соль Portable Vault.");
        }

        if (mode == VaultMode.Windows && salt.Length != 0)
        {
            throw new InvalidDataException("Windows Vault содержит неожиданные параметры ключа.");
        }

        return salt;
    }

    public static byte[] DerivePortableKey(string password, ReadOnlySpan<byte> salt)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Мастер-пароль не задан.", nameof(password));
        }

        if (salt.Length != PortableSaltSize)
        {
            throw new ArgumentException("Некорректная соль Portable Vault.", nameof(salt));
        }

        using var derive = new Rfc2898DeriveBytes(
            password,
            salt.ToArray(),
            Pbkdf2Iterations,
            HashAlgorithmName.SHA256);
        return derive.GetBytes(KeySize);
    }

    public static byte[] CreatePortableSalt()
    {
        return RandomNumberGenerator.GetBytes(PortableSaltSize);
    }

    public static byte[] CreateRandomKey()
    {
        return RandomNumberGenerator.GetBytes(KeySize);
    }

    public static void EncryptToFile(
        string path,
        ReadOnlySpan<byte> plaintext,
        ReadOnlySpan<byte> key,
        VaultMode mode,
        ReadOnlySpan<byte> salt)
    {
        ValidateKey(key);
        ValidateMode(mode);

        if (mode == VaultMode.Portable && salt.Length != PortableSaltSize)
        {
            throw new ArgumentException("Portable Vault requires a 16-byte salt.", nameof(salt));
        }

        if (mode == VaultMode.Windows && salt.Length != 0)
        {
            throw new ArgumentException("Windows Vault must not contain a password salt.", nameof(salt));
        }

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var tag = new byte[TagSize];
        var ciphertext = new byte[plaintext.Length];
        var aad = BuildAad(mode, salt);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plaintext, ciphertext, tag, aad);

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.Write(Magic);
            stream.WriteByte((byte)mode);
            stream.WriteByte((byte)salt.Length);
            stream.Write(salt);
            stream.Write(nonce);
            stream.Write(tag);

            Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(lengthBytes, ciphertext.Length);
            stream.Write(lengthBytes);
            stream.Write(ciphertext);
            stream.Flush(flushToDisk: true);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(ciphertext);
        }
    }

    public static byte[] DecryptFile(
        string path,
        ReadOnlySpan<byte> key,
        out VaultMode mode,
        out byte[] salt)
    {
        ValidateKey(key);

        using var stream = OpenCheckedRead(path);
        ReadAndValidateMagic(stream);

        var modeValue = stream.ReadByte();
        if (modeValue < 0)
        {
            throw new InvalidDataException("Повреждён заголовок зашифрованной истории.");
        }

        mode = (VaultMode)modeValue;
        ValidateMode(mode);

        var saltLength = stream.ReadByte();
        if (saltLength < 0 || saltLength > 64)
        {
            throw new InvalidDataException("Некорректная соль зашифрованной истории.");
        }

        salt = new byte[saltLength];
        ReadExactly(stream, salt);

        if (mode == VaultMode.Portable && salt.Length != PortableSaltSize)
        {
            throw new InvalidDataException("Некорректная соль Portable Vault.");
        }

        if (mode == VaultMode.Windows && salt.Length != 0)
        {
            throw new InvalidDataException("Windows Vault содержит неожиданные параметры ключа.");
        }

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        ReadExactly(stream, nonce);
        ReadExactly(stream, tag);

        Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
        ReadExactly(stream, lengthBytes);
        var ciphertextLength = BinaryPrimitives.ReadInt32LittleEndian(lengthBytes);

        if (ciphertextLength < 0 || ciphertextLength > MaxVaultFileSize)
        {
            throw new InvalidDataException("Некорректный размер зашифрованной истории.");
        }

        if (stream.Length - stream.Position != ciphertextLength)
        {
            throw new InvalidDataException("Размер зашифрованной истории не совпадает с заголовком.");
        }

        var ciphertext = new byte[ciphertextLength];
        var plaintext = new byte[ciphertextLength];
        ReadExactly(stream, ciphertext);
        var aad = BuildAad(mode, salt);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Decrypt(nonce, ciphertext, tag, plaintext, aad);
            return plaintext;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(ciphertext);
        }
    }

    public static List<ClipboardEntry> DeserializeEntries(ReadOnlySpan<byte> plaintext, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<List<ClipboardEntry>>(plaintext, options) ?? [];
    }

    public static byte[] SerializeEntries(IEnumerable<ClipboardEntry> entries, JsonSerializerOptions options)
    {
        return JsonSerializer.SerializeToUtf8Bytes(entries, options);
    }

    private static FileStream OpenCheckedRead(string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists)
        {
            throw new FileNotFoundException("Файл зашифрованной истории не найден.", path);
        }

        if (info.Length <= 0 || info.Length > MaxVaultFileSize)
        {
            throw new InvalidDataException("Некорректный размер зашифрованной истории.");
        }

        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private static byte[] BuildAad(VaultMode mode, ReadOnlySpan<byte> salt)
    {
        var aad = new byte[AadPrefix.Length + 2 + salt.Length];
        AadPrefix.CopyTo(aad, 0);
        aad[AadPrefix.Length] = (byte)mode;
        aad[AadPrefix.Length + 1] = (byte)salt.Length;
        salt.CopyTo(aad.AsSpan(AadPrefix.Length + 2));
        return aad;
    }

    private static void ReadAndValidateMagic(Stream stream)
    {
        Span<byte> magic = stackalloc byte[Magic.Length];
        ReadExactly(stream, magic);
        ValidateMagic(magic);
    }

    private static void ValidateMagic(ReadOnlySpan<byte> actual)
    {
        if (!actual.SequenceEqual(Magic))
        {
            throw new InvalidDataException("Неизвестный формат зашифрованной истории.");
        }
    }

    private static void ValidateMode(VaultMode mode)
    {
        if (mode is not VaultMode.Windows and not VaultMode.Portable)
        {
            throw new InvalidDataException("Неизвестный режим хранилища.");
        }
    }

    private static void ValidateKey(ReadOnlySpan<byte> key)
    {
        if (key.Length != KeySize)
        {
            throw new CryptographicException("Некорректный ключ хранилища.");
        }
    }

    private static void ReadExactly(Stream stream, Span<byte> buffer)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var read = stream.Read(buffer[offset..]);
            if (read <= 0)
            {
                throw new EndOfStreamException("Зашифрованная история неожиданно обрывается.");
            }

            offset += read;
        }
    }

    private static void ReadExactly(Stream stream, byte[] buffer)
    {
        ReadExactly(stream, buffer.AsSpan());
    }
}
