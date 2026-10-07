using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GeniaClipboard;

internal static class WindowsDpapi
{
    private const uint CryptProtectUiForbidden = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct DataBlob
    {
        public int Length;
        public IntPtr Data;
    }

    [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptProtectData(
        ref DataBlob dataIn,
        string? description,
        IntPtr optionalEntropy,
        IntPtr reserved,
        IntPtr promptStruct,
        uint flags,
        out DataBlob dataOut);

    [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptUnprotectData(
        ref DataBlob dataIn,
        IntPtr description,
        IntPtr optionalEntropy,
        IntPtr reserved,
        IntPtr promptStruct,
        uint flags,
        out DataBlob dataOut);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LocalFree(IntPtr memory);

    public static byte[] Protect(ReadOnlySpan<byte> plaintext)
    {
        return Transform(plaintext, protect: true);
    }

    public static byte[] Unprotect(ReadOnlySpan<byte> protectedData)
    {
        return Transform(protectedData, protect: false);
    }

    private static byte[] Transform(ReadOnlySpan<byte> input, bool protect)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Windows DPAPI is only available on Windows.");
        }

        var inputBytes = input.ToArray();
        var inputPointer = IntPtr.Zero;
        var output = default(DataBlob);

        try
        {
            inputPointer = Marshal.AllocHGlobal(inputBytes.Length);
            Marshal.Copy(inputBytes, 0, inputPointer, inputBytes.Length);
            var inputBlob = new DataBlob
            {
                Length = inputBytes.Length,
                Data = inputPointer
            };

            var success = protect
                ? CryptProtectData(
                    ref inputBlob,
                    "GeniaClipboard vault key",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    CryptProtectUiForbidden,
                    out output)
                : CryptUnprotectData(
                    ref inputBlob,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    CryptProtectUiForbidden,
                    out output);

            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var result = new byte[output.Length];
            Marshal.Copy(output.Data, result, 0, result.Length);
            return result;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(inputBytes);

            if (inputPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(inputPointer);
            }

            if (output.Data != IntPtr.Zero)
            {
                LocalFree(output.Data);
            }
        }
    }
}
