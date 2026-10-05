using System.Text;

namespace GeniaClipboard;

internal sealed class TextJournalService
{
    private readonly string _journalDirectory;

    public TextJournalService()
    {
        _journalDirectory = Path.Combine(AppContext.BaseDirectory, "Data", "Journal");
    }

    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (!value)
            {
                LastError = null;
            }
        }
    }

    public string? LastError { get; private set; }

    public string JournalDirectory => _journalDirectory;

    public bool Append(ClipboardEntry entry)
    {
        if (!Enabled)
        {
            return true;
        }

        try
        {
            Directory.CreateDirectory(_journalDirectory);
            var local = entry.CopiedAt.LocalDateTime;
            var path = Path.Combine(_journalDirectory, $"GeniaClipboard_{local:yyyy-MM-dd}.txt");

            using var writer = new StreamWriter(
                path,
                true,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            writer.Write('[');
            writer.Write(local.ToString("HH:mm:ss"));
            writer.WriteLine("]");
            writer.WriteLine(entry.Text);
            writer.WriteLine();

            LastError = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            LastError = $"TXT-журнал не записан: {ex.Message}";
            return false;
        }
    }
}
