using System.Diagnostics;

namespace GeniaClipboard;

internal sealed record ClipboardSourceInfo(
    string? ProcessName,
    string? WindowTitle,
    bool IsOwnProcess)
{
    public static ClipboardSourceInfo Capture()
    {
        try
        {
            var window = NativeMethods.GetForegroundWindow();
            if (window == IntPtr.Zero)
            {
                return new ClipboardSourceInfo(null, null, false);
            }

            _ = NativeMethods.GetWindowThreadProcessId(window, out var processId);
            if (processId == 0)
            {
                return new ClipboardSourceInfo(null, null, false);
            }

            using var process = Process.GetProcessById(checked((int)processId));
            return new ClipboardSourceInfo(
                process.ProcessName,
                process.MainWindowTitle,
                processId == Environment.ProcessId);
        }
        catch
        {
            return new ClipboardSourceInfo(null, null, false);
        }
    }

    public bool IsExcluded(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(ProcessName))
        {
            return false;
        }

        return settings.ExcludedProcesses.Any(value =>
            string.Equals(value, ProcessName, StringComparison.OrdinalIgnoreCase));
    }
}
