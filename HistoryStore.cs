using System.Security.Cryptography;
using System.Text.Json;

namespace GeniaClipboard;

internal sealed class HistoryStore : IDisposable
{
    internal const int MaxTextLength = 1_000_000;
    private const long MaxLegacyHistoryFileSize = 64L * 1024 * 1024;

    private readonly string _dataDirectory;
    private readonly string _vaultPath;
    private readonly string _keyPath;
    private readonly string _legacyHistoryPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    private byte[]? _key;
    private byte[] _portableSalt = [];
    private int _historyLimit;
    private int _retentionDays;
    private bool _disposed;

    private HistoryStore(AppSettings settings, string? portablePassword)
    {
        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        _vaultPath = Path.Combine(_dataDirectory, "history.gch");
        _keyPath = Path.Combine(_dataDirectory, "history.key");
        _legacyHistoryPath = Path.Combine(_dataDirectory, "history.json");

        _historyLimit = settings.HistoryLimit;
        _retentionDays = settings.RetentionDays;

        Items = [];
        Initialize(settings, portablePassword);

        if (settings.PrivateSessionOnStart)
        {
            BeginPrivateSession();
        }
    }

    private HistoryStore(AppSettings settings, VaultMode underlyingMode, bool lockedPrivateSession)
    {
        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        _vaultPath = Path.Combine(_dataDirectory, "history.gch");
        _keyPath = Path.Combine(_dataDirectory, "history.key");
        _legacyHistoryPath = Path.Combine(_dataDirectory, "history.json");

        _historyLimit = settings.HistoryLimit;
        _retentionDays = settings.RetentionDays;
        Items = [];
        VaultMode = underlyingMode;
        IsPrivateSession = lockedPrivateSession;
    }

    public List<ClipboardEntry> Items { get; }

    public VaultMode VaultMode { get; private set; } = VaultMode.Windows;

    public bool IsPrivateSession { get; private set; }

    public bool CanAccessPersistentVault => _key is not null;

    public string? LastError { get; private set; }

    public string? MigrationWarning { get; private set; }

    public static VaultMode ProbeVaultMode()
    {
        var vaultPath = Path.Combine(AppContext.BaseDirectory, "Data", "history.gch");
        return File.Exists(vaultPath)
            ? EncryptedHistoryCodec.ProbeMode(vaultPath)
            : VaultMode.Windows;
    }

    public static HistoryStore CreateLockedPrivateSession(
        AppSettings settings,
        VaultMode underlyingMode)
    {
        return new HistoryStore(settings, underlyingMode, lockedPrivateSession: true);
    }

    public static bool TryOpen(
        AppSettings settings,
        string? portablePassword,
        out HistoryStore? store,
        out string? error)
    {
        store = null;
        error = null;

        try
        {
            store = new HistoryStore(settings, portablePassword);
            return true;
        }
        catch (CryptographicException)
        {
            error = "Не удалось расшифровать историю. Проверьте мастер-пароль или Windows-профиль.";
            return false;
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or JsonException
            or NotSupportedException
            or ArgumentException)
        {
            error = $"Не удалось открыть хранилище GeniaClipboard: {ex.Message}";
            return false;
        }
    }

    public bool AddOrPromote(
        string text,
        string? sourceProcess,
        string? sourceWindowTitle,
        bool isSensitive,
        int sensitiveExpireSeconds,
        out ClipboardEntry? newEntry)
    {
        ThrowIfDisposed();
        newEntry = null;

        if (string.IsNullOrWhiteSpace(text) || text.Length > MaxTextLength)
        {
            return false;
        }

        var now = DateTimeOffset.Now;
        var existing = Items.FirstOrDefault(item =>
            string.Equals(item.Text, text, StringComparison.Ordinal));

        if (existing is not null)
        {
            existing.CopiedAt = now;
            existing.SourceProcess = sourceProcess;
            existing.SourceWindowTitle = sourceWindowTitle;

            if (isSensitive)
            {
                existing.IsSensitive = true;
                if (!existing.IsPinned && sensitiveExpireSeconds > 0)
                {
                    existing.ExpiresAt = now.AddSeconds(sensitiveExpireSeconds);
                }
            }

            Items.Remove(existing);
            Items.Insert(0, existing);
        }
        else
        {
            newEntry = new ClipboardEntry
            {
                Text = text,
                CopiedAt = now,
                SourceProcess = sourceProcess,
                SourceWindowTitle = sourceWindowTitle,
                IsSensitive = isSensitive,
                ExpiresAt = isSensitive && sensitiveExpireSeconds > 0
                    ? now.AddSeconds(sensitiveExpireSeconds)
                    : null
            };
            Items.Insert(0, newEntry);
        }

        Prune();
        Save();
        return true;
    }

    public bool UpdateText(ClipboardEntry entry, string text)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(text) || text.Length > MaxTextLength)
        {
            return false;
        }

        var duplicate = Items.FirstOrDefault(item =>
            item.Id != entry.Id &&
            string.Equals(item.Text, text, StringComparison.Ordinal));

        if (duplicate is not null)
        {
            entry.IsPinned |= duplicate.IsPinned;
            entry.IsSensitive |= duplicate.IsSensitive;
            entry.ExpiresAt ??= duplicate.ExpiresAt;
            Items.Remove(duplicate);
        }

        entry.Text = text;
        entry.CopiedAt = DateTimeOffset.Now;
        Items.Remove(entry);
        Items.Insert(0, entry);
        Prune();
        Save();
        return true;
    }

    public void TogglePinned(ClipboardEntry entry)
    {
        ThrowIfDisposed();
        entry.IsPinned = !entry.IsPinned;
        entry.CopiedAt = DateTimeOffset.Now;

        if (entry.IsPinned)
        {
            entry.ExpiresAt = null;
        }

        Save();
    }

    public void RemoveMany(IEnumerable<ClipboardEntry> entries)
    {
        ThrowIfDisposed();

        var ids = entries.Select(entry => entry.Id).ToHashSet();
        if (ids.Count == 0)
        {
            return;
        }

        Items.RemoveAll(item => ids.Contains(item.Id));
        Save();
    }

    public int RemoveExpired()
    {
        ThrowIfDisposed();

        var now = DateTimeOffset.Now;
        var removed = Items.RemoveAll(item =>
            !item.IsPinned &&
            item.ExpiresAt is { } expiresAt &&
            expiresAt <= now);

        if (removed > 0)
        {
            Save();
        }

        return removed;
    }

    public void Clear()
    {
        ThrowIfDisposed();
        Items.Clear();
        Save();
    }

    public void ApplyPolicy(int historyLimit, int retentionDays)
    {
        ThrowIfDisposed();
        _historyLimit = Math.Clamp(historyLimit, 20, 100_000);
        _retentionDays = Math.Clamp(retentionDays, 0, 3650);
        Prune();
        Save();
    }

    public bool ChangeVaultMode(VaultMode newMode, string? newPortablePassword)
    {
        ThrowIfDisposed();

        if (IsPrivateSession)
        {
            LastError = "Режим хранилища нельзя менять во время Private Session.";
            return false;
        }

        if (newMode is not VaultMode.Windows and not VaultMode.Portable)
        {
            LastError = "Неизвестный режим хранилища.";
            return false;
        }

        if (newMode == VaultMode.Portable &&
            VaultMode == VaultMode.Portable &&
            string.IsNullOrEmpty(newPortablePassword))
        {
            LastError = null;
            return true;
        }

        if (newMode == VaultMode.Windows && VaultMode == VaultMode.Windows)
        {
            LastError = null;
            return true;
        }

        if (newMode == VaultMode.Portable &&
            (newPortablePassword is null || newPortablePassword.Length < 10))
        {
            LastError = "Для Portable Vault задайте мастер-пароль не короче 10 символов.";
            return false;
        }

        byte[]? newKey = null;
        byte[] newSalt = [];

        try
        {
            if (newMode == VaultMode.Windows)
            {
                newKey = EncryptedHistoryCodec.CreateRandomKey();
            }
            else
            {
                newSalt = EncryptedHistoryCodec.CreatePortableSalt();
                newKey = EncryptedHistoryCodec.DerivePortableKey(newPortablePassword!, newSalt);
            }

            var temporaryVaultPath = _vaultPath + ".rekey";
            WriteVaultFile(temporaryVaultPath, newKey, newMode, newSalt);
            VerifyVaultFile(temporaryVaultPath, newKey);

            if (newMode == VaultMode.Windows)
            {
                PersistWindowsKey(newKey);
            }

            File.Move(temporaryVaultPath, _vaultPath, true);

            if (newMode == VaultMode.Portable)
            {
                TryDeleteFile(_keyPath);
            }

            if (_key is not null)
            {
                CryptographicOperations.ZeroMemory(_key);
            }

            _key = newKey;
            newKey = null;
            CryptographicOperations.ZeroMemory(_portableSalt);
            _portableSalt = newSalt;
            newSalt = [];
            VaultMode = newMode;
            LastError = null;
            return true;
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or CryptographicException
            or ArgumentException)
        {
            LastError = $"Не удалось изменить режим хранилища: {ex.Message}";
            return false;
        }
        finally
        {
            if (newKey is not null)
            {
                CryptographicOperations.ZeroMemory(newKey);
            }

            CryptographicOperations.ZeroMemory(newSalt);
            TryDeleteFile(_vaultPath + ".rekey");
        }
    }

    public void BeginPrivateSession()
    {
        ThrowIfDisposed();

        if (IsPrivateSession)
        {
            return;
        }

        IsPrivateSession = true;
        Items.Clear();
        LastError = null;
    }

    public bool EndPrivateSession()
    {
        ThrowIfDisposed();

        if (!IsPrivateSession)
        {
            return true;
        }

        if (_key is null)
        {
            LastError = "Постоянный vault не разблокирован. Перезапустите GeniaClipboard и разблокируйте vault мастер-паролем.";
            return false;
        }

        try
        {
            var entries = LoadEncryptedEntries();
            Items.Clear();
            Items.AddRange(entries);
            IsPrivateSession = false;
            Prune();
            LastError = null;
            return true;
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or CryptographicException
            or JsonException)
        {
            LastError = $"Не удалось вернуться к постоянной истории: {ex.Message}";
            return false;
        }
    }

    public void Flush()
    {
        ThrowIfDisposed();
        Save();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_key is not null)
        {
            CryptographicOperations.ZeroMemory(_key);
            _key = null;
        }

        CryptographicOperations.ZeroMemory(_portableSalt);
        _portableSalt = [];
        Items.Clear();
        GC.SuppressFinalize(this);
    }

    private void Initialize(AppSettings settings, string? portablePassword)
    {
        if (File.Exists(_vaultPath))
        {
            VaultMode = EncryptedHistoryCodec.ProbeMode(_vaultPath);
            UnlockExistingVault(portablePassword);
            Items.AddRange(LoadEncryptedEntries());
            NormalizeEntries();
            DeleteLegacyPlaintextIfPresent();
            return;
        }

        VaultMode = settings.PreferredVaultMode;
        if (VaultMode == VaultMode.Portable)
        {
            if (string.IsNullOrEmpty(portablePassword) || portablePassword.Length < 10)
            {
                throw new CryptographicException("Portable Vault requires a master password.");
            }

            _portableSalt = EncryptedHistoryCodec.CreatePortableSalt();
            _key = EncryptedHistoryCodec.DerivePortableKey(portablePassword, _portableSalt);
        }
        else
        {
            VaultMode = VaultMode.Windows;
            _key = EncryptedHistoryCodec.CreateRandomKey();
            PersistWindowsKey(_key);
        }

        var legacyEntries = LoadLegacyEntries();
        Items.AddRange(legacyEntries);
        NormalizeEntries();

        if (!Save())
        {
            throw new IOException(LastError ?? "Не удалось создать зашифрованное хранилище.");
        }

        DeleteLegacyPlaintextIfPresent();
    }

    private void UnlockExistingVault(string? portablePassword)
    {
        if (VaultMode == VaultMode.Windows)
        {
            if (!File.Exists(_keyPath))
            {
                throw new InvalidDataException("Файл ключа Windows Vault отсутствует.");
            }

            var protectedKey = File.ReadAllBytes(_keyPath);
            if (protectedKey.Length is <= 0 or > 16_384)
            {
                throw new InvalidDataException("Файл ключа Windows Vault повреждён.");
            }

            _key = WindowsDpapi.Unprotect(protectedKey);
            CryptographicOperations.ZeroMemory(protectedKey);

            if (_key.Length != 32)
            {
                throw new CryptographicException("Windows Vault вернул ключ неверного размера.");
            }

            _portableSalt = [];
            return;
        }

        if (string.IsNullOrEmpty(portablePassword))
        {
            throw new CryptographicException("Portable Vault заблокирован.");
        }

        _portableSalt = EncryptedHistoryCodec.ReadSalt(_vaultPath);
        _key = EncryptedHistoryCodec.DerivePortableKey(portablePassword, _portableSalt);
    }

    private List<ClipboardEntry> LoadEncryptedEntries()
    {
        if (_key is null)
        {
            throw new CryptographicException("Хранилище не разблокировано.");
        }

        var plaintext = EncryptedHistoryCodec.DecryptFile(
            _vaultPath,
            _key,
            out var actualMode,
            out var actualSalt);

        try
        {
            if (actualMode != VaultMode)
            {
                throw new InvalidDataException("Режим хранилища изменён без корректной миграции.");
            }

            if (VaultMode == VaultMode.Portable &&
                !actualSalt.AsSpan().SequenceEqual(_portableSalt))
            {
                throw new InvalidDataException("Параметры Portable Vault не совпадают.");
            }

            return EncryptedHistoryCodec.DeserializeEntries(plaintext, _jsonOptions);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plaintext);
            CryptographicOperations.ZeroMemory(actualSalt);
        }
    }

    private List<ClipboardEntry> LoadLegacyEntries()
    {
        if (!File.Exists(_legacyHistoryPath))
        {
            return [];
        }

        var info = new FileInfo(_legacyHistoryPath);
        if (info.Length > MaxLegacyHistoryFileSize)
        {
            throw new InvalidDataException("Старый history.json слишком большой для безопасной миграции.");
        }

        var json = File.ReadAllText(_legacyHistoryPath);
        return JsonSerializer.Deserialize<List<ClipboardEntry>>(json, _jsonOptions) ?? [];
    }

    private void NormalizeEntries()
    {
        Items.RemoveAll(entry =>
            string.IsNullOrWhiteSpace(entry.Text) ||
            entry.Text.Length > MaxTextLength);

        foreach (var entry in Items)
        {
            if (entry.Id == Guid.Empty)
            {
                entry.Id = Guid.NewGuid();
            }
        }

        var ordered = Items
            .OrderByDescending(entry => entry.IsPinned)
            .ThenByDescending(entry => entry.CopiedAt)
            .ToList();

        Items.Clear();
        Items.AddRange(ordered);
        Prune();
    }

    private void Prune()
    {
        var now = DateTimeOffset.Now;

        Items.RemoveAll(item =>
            !item.IsPinned &&
            item.ExpiresAt is { } expiresAt &&
            expiresAt <= now);

        if (_retentionDays > 0)
        {
            var cutoff = now.AddDays(-_retentionDays);
            Items.RemoveAll(item => !item.IsPinned && item.CopiedAt < cutoff);
        }

        var overflow = Items
            .Where(item => !item.IsPinned)
            .OrderByDescending(item => item.CopiedAt)
            .Skip(_historyLimit)
            .Select(item => item.Id)
            .ToHashSet();

        if (overflow.Count > 0)
        {
            Items.RemoveAll(item => overflow.Contains(item.Id));
        }
    }

    private bool Save()
    {
        if (IsPrivateSession)
        {
            LastError = null;
            return true;
        }

        if (_key is null)
        {
            LastError = "Хранилище не разблокировано; запись отменена.";
            return false;
        }

        try
        {
            Directory.CreateDirectory(_dataDirectory);
            Prune();

            var temporaryPath = _vaultPath + ".tmp";
            WriteVaultFile(temporaryPath, _key, VaultMode, _portableSalt);
            VerifyVaultFile(temporaryPath, _key);
            File.Move(temporaryPath, _vaultPath, true);
            LastError = null;
            return true;
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or CryptographicException
            or JsonException
            or NotSupportedException)
        {
            LastError = $"История не сохранена: {ex.Message}";
            return false;
        }
        finally
        {
            TryDeleteFile(_vaultPath + ".tmp");
        }
    }

    private void WriteVaultFile(
        string path,
        ReadOnlySpan<byte> key,
        VaultMode mode,
        ReadOnlySpan<byte> salt)
    {
        var orderedItems = Items
            .OrderByDescending(item => item.IsPinned)
            .ThenByDescending(item => item.CopiedAt)
            .ToList();

        var plaintext = EncryptedHistoryCodec.SerializeEntries(orderedItems, _jsonOptions);
        try
        {
            EncryptedHistoryCodec.EncryptToFile(path, plaintext, key, mode, salt);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plaintext);
        }
    }

    private void VerifyVaultFile(string path, ReadOnlySpan<byte> key)
    {
        var plaintext = EncryptedHistoryCodec.DecryptFile(path, key, out _, out var salt);
        try
        {
            _ = EncryptedHistoryCodec.DeserializeEntries(plaintext, _jsonOptions);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plaintext);
            CryptographicOperations.ZeroMemory(salt);
        }
    }

    private void PersistWindowsKey(ReadOnlySpan<byte> key)
    {
        Directory.CreateDirectory(_dataDirectory);

        var protectedKey = WindowsDpapi.Protect(key);
        var temporaryKeyPath = _keyPath + ".tmp";

        try
        {
            using (var stream = new FileStream(
                       temporaryKeyPath,
                       FileMode.Create,
                       FileAccess.Write,
                       FileShare.None))
            {
                stream.Write(protectedKey);
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporaryKeyPath, _keyPath, true);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(protectedKey);
            TryDeleteFile(temporaryKeyPath);
        }
    }

    private void DeleteLegacyPlaintextIfPresent()
    {
        if (!File.Exists(_legacyHistoryPath))
        {
            return;
        }

        try
        {
            File.Delete(_legacyHistoryPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MigrationWarning =
                $"История уже зашифрована, но старый незашифрованный history.json не удалось удалить: {ex.Message}";
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Cleanup is best-effort. The main operation reports its own errors.
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
