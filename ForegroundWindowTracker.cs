using System.Runtime.InteropServices;
using System.Text;

namespace GeniaClipboard;

/// <summary>
/// Tracks real foreground applications independently of how the clipboard
/// window is opened (hotkey, tray, taskbar). The Windows shell/taskbar and
/// GeniaClipboard itself are not valid paste destinations.
/// </summary>
internal sealed class ForegroundWindowTracker : IDisposable
{
    private const uint EventSystemForeground = 0x0003;
    private const uint WinEventOutOfContext = 0x0000;
    private const uint WinEventSkipOwnProcess = 0x0002;
    private const long MaxTargetAgeMilliseconds = 120_000;

    private delegate void WinEventProc(
        IntPtr eventHook,
        uint eventType,
        IntPtr window,
        int objectId,
        int childId,
        uint eventThread,
        uint eventTime);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWinEventHook(
        uint eventMin,
        uint eventMax,
        IntPtr module,
        WinEventProc callback,
        uint processId,
        uint threadId,
        uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWinEvent(IntPtr eventHook);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(
        IntPtr window,
        StringBuilder className,
        int maximumCount);

    private readonly WinEventProc _callback;
    private IntPtr _hook;
    private IntPtr _lastWindow;
    private uint _lastProcessId;
    private long _lastSeenAt;
    private bool _disposed;

    public ForegroundWindowTracker()
    {
        _callback = ForegroundChanged;
        _hook = SetWinEventHook(
            EventSystemForeground,
            EventSystemForeground,
            IntPtr.Zero,
            _callback,
            0,
            0,
            WinEventOutOfContext | WinEventSkipOwnProcess);

        ObserveCurrentForeground();
    }

    public bool TrackingAvailable => _hook != IntPtr.Zero;

    public void ObserveCurrentForeground()
    {
        if (!_disposed)
        {
            Observe(NativeMethods.GetForegroundWindow());
        }
    }

    public bool TryGetTarget(out IntPtr window, out uint processId)
    {
        ObserveCurrentForeground();
        window = _lastWindow;
        processId = _lastProcessId;

        if (window == IntPtr.Zero ||
            Environment.TickCount64 - _lastSeenAt > MaxTargetAgeMilliseconds ||
            !IsSameWindow(window, processId))
        {
            window = IntPtr.Zero;
            processId = 0;
            return false;
        }

        return true;
    }

    public bool IsSameWindow(IntPtr window, uint processId)
    {
        if (_disposed ||
            window == IntPtr.Zero ||
            processId == 0 ||
            processId == (uint)Environment.ProcessId ||
            !NativeMethods.IsWindow(window) ||
            IsShellWindow(window))
        {
            return false;
        }

        return NativeMethods.GetWindowThreadProcessId(window, out var actualProcessId) != 0 &&
               actualProcessId == processId;
    }

    public void Clear()
    {
        _lastWindow = IntPtr.Zero;
        _lastProcessId = 0;
        _lastSeenAt = 0;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_hook != IntPtr.Zero)
        {
            _ = UnhookWinEvent(_hook);
            _hook = IntPtr.Zero;
        }

        Clear();
        GC.SuppressFinalize(this);
    }

    private void ForegroundChanged(
        IntPtr eventHook,
        uint eventType,
        IntPtr window,
        int objectId,
        int childId,
        uint eventThread,
        uint eventTime)
    {
        if (_disposed ||
            eventType != EventSystemForeground ||
            objectId != 0 ||
            childId != 0 ||
            NativeMethods.GetForegroundWindow() != window)
        {
            return;
        }

        Observe(window);
    }

    private void Observe(IntPtr window)
    {
        if (window == IntPtr.Zero ||
            !NativeMethods.IsWindow(window) ||
            IsShellWindow(window) ||
            NativeMethods.GetWindowThreadProcessId(window, out var processId) == 0 ||
            processId == 0 ||
            processId == (uint)Environment.ProcessId)
        {
            return;
        }

        _lastWindow = window;
        _lastProcessId = processId;
        _lastSeenAt = Environment.TickCount64;
    }

    private static bool IsShellWindow(IntPtr window)
    {
        var name = new StringBuilder(128);
        if (GetClassName(window, name, name.Capacity) == 0)
        {
            return false;
        }

        return name.ToString() is
            "Shell_TrayWnd" or
            "Shell_SecondaryTrayWnd" or
            "NotifyIconOverflowWindow" or
            "Progman" or
            "WorkerW";
    }
}
