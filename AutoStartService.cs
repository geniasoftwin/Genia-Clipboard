using Microsoft.Win32;

namespace GeniaClipboard;

internal static class AutoStartService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "GeniaClipboard";

    public static bool SetEnabled(bool enabled, out string? error)
    {
        error = null;

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

            if (key is null)
            {
                error = "Не удалось открыть раздел автозапуска текущего пользователя.";
                return false;
            }

            if (enabled)
            {
                var executable = Application.ExecutablePath;
                key.SetValue(ValueName, $"\"{executable}\"", RegistryValueKind.String);
            }
            else
            {
                key.DeleteValue(ValueName, throwOnMissingValue: false);
            }

            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or System.Security.SecurityException)
        {
            error = $"Не удалось изменить автозапуск: {ex.Message}";
            return false;
        }
    }
}
