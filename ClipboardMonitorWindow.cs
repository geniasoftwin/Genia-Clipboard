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
    private HotKeyDefinition _currentHotKey =
        new(NativeMethods.ModControl | NativeMethods.ModShift, Keys.V);

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

    public HotKeyDefinition CurrentHotKey => _currentHotKey;

    public void Start(HotKeyDefinition hotKey)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ClipboardMonitorWindow));
        }

        if (!_clipboardListenerRegistered)
        {
            _clipboardListenerRegistered = NativeMethods.AddClipboardFormatListener(Handle);
        }

        _currentHotKey = hotKey;
        _hotKeyRegistered = Register(hotKey);
    }

    public bool TrySetHotKey(HotKeyDefinition hotKey)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ClipboardMonitorWindow));
        }

        if (hotKey == _currentHotKey && _hotKeyRegistered)
        {
            return true;
        }

        var previous = _currentHotKey;
        var previousWasRegistered = _hotKeyRegistered;

        if (_hotKeyRegistered)
        {
            NativeMethods.UnregisterHotKey(Handle, HotKeyId);
            _hotKeyRegistered = false;
        }

        if (Register(hotKey))
        {
            _currentHotKey = hotKey;
            _hotKeyRegistered = true;
            return true;
        }

        _currentHotKey = previous;
        _hotKeyRegistered = previousWasRegistered && Register(previous);
        return false;
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

    private bool Register(HotKeyDefinition hotKey)
    {
        if (hotKey.Key == Keys.None)
        {
            return false;
        }

        return NativeMethods.RegisterHotKey(
            Handle,
            HotKeyId,
            hotKey.Modifiers | NativeMethods.ModNoRepeat,
            (int)hotKey.Key);
    }
}
