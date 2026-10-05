using System.Text.Json;

namespace GeniaClipboard;

internal sealed class HistoryStore
{
    private const int MaxUnpinnedItems = 500;
    internal const int MaxTextLength = 1_000_000;
    private const long MaxHistoryFileSize = 64L * 1024 * 1024;

    private readonly string _dataDirectory;
    private readonly string _historyPath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false
    };

    public HistoryStore()
    {
        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        _historyPath = Path.Combine(_dataDirectory, "history.json");
        Items = Load();
    }

    public List<ClipboardEntry> Items { get; }

    public string? LastError { get; private set; }

    public bool AddOrPromote(string text, out ClipboardEntry? newEntry)
    {
        newEntry = null;
        if (string.IsNullOrWhiteSpace(text) || text.Length > MaxTextLength)
        {
            return false;
        }

        var existing = Items.FirstOrDefault(item =>
            string.Equals(item.Text, text, StringComparison.Ordinal));

        if (existing is not null)
        {
            existing.CopiedAt = DateTimeOffset.Now;
            Items.Remove(existing);
            Items.Insert(0, existing);
        }
        else
        {
            newEntry = new ClipboardEntry
            {
                Text = text,
                CopiedAt = DateTimeOffset.Now
            };
            Items.Insert(0, newEntry);
        }

        Prune();
        Save();
        return true;
    }

    public void TogglePinned(ClipboardEntry entry)
    {
        entry.IsPinned = !entry.IsPinned;
        entry.CopiedAt = DateTimeOffset.Now;
        Save();
    }

    public void Remove(ClipboardEntry entry)
    {
        RemoveMany([entry]);
    }

    public void RemoveMany(IEnumerable<ClipboardEntry> entries)
    {
        var ids = entries.Select(entry => entry.Id).ToHashSet();
        if (ids.Count == 0)
        {
            return;
        }

        Items.RemoveAll(item => ids.Contains(item.Id));
        Save();
    }

    public void Clear()
    {
        Items.Clear();
        Save();
    }

    public void Flush() => Save();

    private List<ClipboardEntry> Load()
    {
        try
        {
            if (!File.Exists(_historyPath))
            {
                return [];
            }

            var info = new FileInfo(_historyPath);
            if (info.Length > MaxHistoryFileSize)
            {
                throw new InvalidDataException("Файл истории слишком большой.");
            }

            var json = File.ReadAllText(_historyPath);
            var entries = JsonSerializer.Deserialize<List<ClipboardEntry>>(json, _jsonOptions) ?? [];

            foreach (var entry in entries)
            {
                if (entry.Id == Guid.Empty)
                {
                    entry.Id = Guid.NewGuid();
                }
            }

            return entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Text) && entry.Text.Length <= MaxTextLength)
                .OrderByDescending(entry => entry.IsPinned)
                .ThenByDescending(entry => entry.CopiedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            LastError = $"Не удалось прочитать историю: {ex.Message}";
            return [];
        }
    }

    private void Prune()
    {
        var unpinned = Items
            .Where(item => !item.IsPinned)
            .OrderByDescending(item => item.CopiedAt)
            .Skip(MaxUnpinnedItems)
            .ToList();

        foreach (var entry in unpinned)
        {
            Items.Remove(entry);
        }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(_dataDirectory);
            var temporaryPath = _historyPath + ".tmp";
            var orderedItems = Items
                .OrderByDescending(item => item.IsPinned)
                .ThenByDescending(item => item.CopiedAt)
                .ToList();
            using (var stream = new FileStream(
                       temporaryPath,
                       FileMode.Create,
                       FileAccess.Write,
                       FileShare.None))
            {
                JsonSerializer.Serialize(stream, orderedItems, _jsonOptions);
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporaryPath, _historyPath, true);
            LastError = null;
        }
        catch (Exception ex)
        {
            LastError = $"История не сохранена: {ex.Message}";
        }
    }
}
