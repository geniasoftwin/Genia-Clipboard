namespace GeniaClipboard;

internal sealed class ClipboardEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CopiedAt { get; set; } = DateTimeOffset.Now;

    public bool IsPinned { get; set; }
}
