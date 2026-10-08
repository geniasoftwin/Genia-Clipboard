namespace GeniaClipboard;

/// <summary>
/// Keeps the native WinForms ListView selection, text rendering and column
/// headers intact, then overlays subtle separators between visible rows.
/// Native GridLines are intentionally disabled: they add vertical lines too.
/// </summary>
internal sealed class SubtleRowListView : ListView
{
    private const int WmPaint = 0x000F;
    private static readonly Color SeparatorColor = Color.FromArgb(235, 239, 243);

    public SubtleRowListView()
    {
        GridLines = false;
    }

    protected override void WndProc(ref Message message)
    {
        var isPaint = message.Msg == WmPaint;
        base.WndProc(ref message);

        if (isPaint && IsHandleCreated && !Disposing &&
            View == View.Details && Items.Count > 1)
        {
            DrawVisibleSeparators();
        }
    }

    private void DrawVisibleSeparators()
    {
        var topItem = TopItem;
        if (topItem is null)
        {
            return;
        }

        var width = ClientSize.Width;
        if (width < 3)
        {
            return;
        }

        using var graphics = CreateGraphics();
        using var pen = new Pen(SeparatorColor);

        // Only inspect visible rows, so drawing remains cheap even with many
        // thousands of history entries.
        for (var index = topItem.Index; index < Items.Count - 1; index++)
        {
            var bounds = Items[index].Bounds;
            if (bounds.Top >= ClientSize.Height)
            {
                break;
            }

            var bottom = bounds.Bottom - 1;
            if (bottom >= 0 && bottom < ClientSize.Height)
            {
                graphics.DrawLine(pen, 1, bottom, width - 2, bottom);
            }
        }
    }
}
