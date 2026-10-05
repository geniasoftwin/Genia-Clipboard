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
    }

    public AppSettings Settings { get; }

    public string? LastError { get; private set; }

    public void Save()
    {
        try
        {
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
}
