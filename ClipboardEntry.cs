namespace GeniaClipboard;

internal sealed class ClipboardEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CopiedAt { get; set; } = DateTimeOffset.Now;

    public bool IsPinned { get; set; }

    public bool IsSensitive { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public string? SourceProcess { get; set; }

    public string? SourceWindowTitle { get; set; }
}
