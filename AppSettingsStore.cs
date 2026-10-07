using System.Text.Json;

namespace GeniaClipboard;

internal sealed class AppSettingsStore
{
    private readonly string _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    public AppSettingsStore()
    {
        _settingsPath = Path.Combine(_dataDirectory, "settings.json");
        Settings = Load();
        Settings.Normalize();
    }

    public AppSettings Settings { get; }

    public string? LastError { get; private set; }

    public void Save()
    {
        try
        {
            Settings.Normalize();
            Directory.CreateDirectory(_dataDirectory);
            var temporaryPath = _settingsPath + ".tmp";
            var json = JsonSerializer.Serialize(Settings, _jsonOptions);
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, _settingsPath, true);
            LastError = null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or NotSupportedException)
        {
            LastError = $"Настройки не сохранены: {ex.Message}";
        }
    }

    private AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettings();
            }

            var info = new FileInfo(_settingsPath);
            if (info.Length > 1_000_000)
            {
                throw new InvalidDataException("Файл настроек слишком большой.");
            }

            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException or NotSupportedException)
        {
            LastError = $"Настройки не прочитаны: {ex.Message}";
            return new AppSettings();
        }
    }
}

internal sealed class AppSettings
{
    public bool AutoJournalEnabled { get; set; }

    public bool AutoStartEnabled { get; set; }

    public int HistoryLimit { get; set; } = 500;

    public int RetentionDays { get; set; }

    public int HotKeyKey { get; set; } = 0x56; // V

    public uint HotKeyModifiers { get; set; } = NativeMethods.ModControl | NativeMethods.ModShift;

    public bool RespectWindowsPrivacyMarkers { get; set; } = true;

    public bool DetectSensitiveText { get; set; } = true;

    public int SensitiveExpireSeconds { get; set; } = 30;

    public int ClipboardAutoClearSeconds { get; set; }

    public bool PrivateSessionOnStart { get; set; }

    public VaultMode PreferredVaultMode { get; set; } = VaultMode.Windows;

    public List<string> ExcludedProcesses { get; set; } = [];

    public void Normalize()
    {
        HistoryLimit = Math.Clamp(HistoryLimit, 20, 100_000);
        RetentionDays = Math.Clamp(RetentionDays, 0, 3650);
        SensitiveExpireSeconds = Math.Clamp(SensitiveExpireSeconds, 0, 86_400);
        ClipboardAutoClearSeconds = Math.Clamp(ClipboardAutoClearSeconds, 0, 86_400);

        if (!Enum.IsDefined(typeof(Keys), HotKeyKey) || HotKeyKey == (int)Keys.None)
        {
            HotKeyKey = (int)Keys.V;
        }

        HotKeyModifiers &=
            NativeMethods.ModAlt |
            NativeMethods.ModControl |
            NativeMethods.ModShift |
            NativeMethods.ModWin;

        if (HotKeyModifiers == 0)
        {
            HotKeyModifiers = NativeMethods.ModControl | NativeMethods.ModShift;
        }

        ExcludedProcesses = ExcludedProcesses
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? value.Trim()[..^4]
                : value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(200)
            .ToList();

        if (PreferredVaultMode is not VaultMode.Windows and not VaultMode.Portable)
        {
            PreferredVaultMode = VaultMode.Windows;
        }
    }
}
