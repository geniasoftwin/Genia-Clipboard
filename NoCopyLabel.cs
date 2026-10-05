namespace GeniaClipboard;

/// <summary>
/// WinForms Label copies its Text to the Windows clipboard on a mouse
/// double-click. For passive UI labels in a clipboard manager this is both
/// surprising and harmful, because it overwrites the user's clipboard.
/// Suppress the label-specific double-click message entirely.
/// </summary>
internal sealed class NoCopyLabel : Label
{
    private const int WmLButtonDoubleClick = 0x0203;

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == WmLButtonDoubleClick)
        {
            return;
        }

        base.WndProc(ref message);
    }
}
