namespace GeniaClipboard;

/// <summary>
/// A permanent message-only window. Unlike a Form handle, this handle is never
/// recreated when the visible window moves between the taskbar and the tray.
/// </summary>
internal sealed class ClipboardMonitorWindow : NativeWindow, IDisposable
{
    private const int HotKeyId = 0x4743;
    private bool _clipboardListenerRegistered;
    private bool _hotKeyRegistered;
    private bool _disposed;

    public ClipboardMonitorWindow()
    {
        CreateHandle(new CreateParams
        {
            Caption = "GeniaClipboard.MessageWindow",
            Parent = NativeMethods.HwndMessage
        });
    }

    public event EventHandler? ClipboardUpdated;

    public event EventHandler? HotKeyPressed;

    public bool ClipboardListenerRegistered => _clipboardListenerRegistered;

    public bool HotKeyRegistered => _hotKeyRegistered;

    public void Start()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ClipboardMonitorWindow));
        }

        if (!_clipboardListenerRegistered)
        {
            _clipboardListenerRegistered = NativeMethods.AddClipboardFormatListener(Handle);
        }

        if (!_hotKeyRegistered)
        {
            _hotKeyRegistered = NativeMethods.RegisterHotKey(
                Handle,
                HotKeyId,
                NativeMethods.ModControl | NativeMethods.ModShift | NativeMethods.ModNoRepeat,
                (int)Keys.V);
        }
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == NativeMethods.WmClipboardUpdate)
        {
            ClipboardUpdated?.Invoke(this, EventArgs.Empty);
        }
        else if (message.Msg == NativeMethods.WmHotKey && message.WParam.ToInt32() == HotKeyId)
        {
            HotKeyPressed?.Invoke(this, EventArgs.Empty);
        }

        base.WndProc(ref message);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_clipboardListenerRegistered)
        {
            NativeMethods.RemoveClipboardFormatListener(Handle);
            _clipboardListenerRegistered = false;
        }

        if (_hotKeyRegistered)
        {
            NativeMethods.UnregisterHotKey(Handle, HotKeyId);
            _hotKeyRegistered = false;
        }

        DestroyHandle();
        GC.SuppressFinalize(this);
    }
}
