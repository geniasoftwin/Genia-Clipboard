namespace GeniaClipboard;

internal sealed class GeniaClipboardApplicationContext : ApplicationContext
{
    private readonly HistoryStore _store;
    private readonly AppSettingsStore _settingsStore;
    private readonly TextJournalService _journal;
    private readonly MainForm _mainForm;
    private readonly NotifyIcon _notifyIcon;
    private readonly ClipboardMonitorWindow _monitorWindow;
    private readonly System.Windows.Forms.Timer _clipboardPollTimer;
    private readonly ToolStripMenuItem _captureMenuItem;
    private bool _isExiting;

    public GeniaClipboardApplicationContext()
    {
        _store = new HistoryStore();
        _settingsStore = new AppSettingsStore();
        _journal = new TextJournalService
        {
            Enabled = _settingsStore.Settings.AutoJournalEnabled
        };
        _mainForm = new MainForm(_store, _journal);

        var openMenuItem = new ToolStripMenuItem("Открыть");
        openMenuItem.Click += (_, _) => _mainForm.ShowWindow(rememberForegroundWindow: false);

        _captureMenuItem = new ToolStripMenuItem("Сохранять скопированное")
        {
            Checked = true,
            CheckOnClick = true
        };
        _captureMenuItem.CheckedChanged += (_, _) =>
        {
            _mainForm.SetCaptureEnabled(_captureMenuItem.Checked);
        };

        var autoJournalMenuItem = new ToolStripMenuItem("Автожурнал TXT")
        {
            Checked = _journal.Enabled,
            CheckOnClick = true
        };
        autoJournalMenuItem.CheckedChanged += (_, _) =>
        {
            _mainForm.SetAutoJournalEnabled(autoJournalMenuItem.Checked);
            _settingsStore.Settings.AutoJournalEnabled = autoJournalMenuItem.Checked;
            _settingsStore.Save();
            if (_settingsStore.LastError is not null)
            {
                MessageBox.Show(
                    _settingsStore.LastError,
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        };

        var clearMenuItem = new ToolStripMenuItem("Очистить историю");
        clearMenuItem.Click += (_, _) => _mainForm.ClearHistoryWithConfirmation();

        var exitMenuItem = new ToolStripMenuItem("Выход");
        exitMenuItem.Click += (_, _) => ExitApplication();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange([
            openMenuItem,
            new ToolStripSeparator(),
            _captureMenuItem,
            autoJournalMenuItem,
            clearMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
        ]);

        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Icon = _mainForm.Icon,
            Text = "GeniaClipboard — Ctrl+Shift+V",
            ContextMenuStrip = menu
        };
        _notifyIcon.MouseDown += (_, _) => _mainForm.RememberForegroundWindow();
        _notifyIcon.DoubleClick += (_, _) => _mainForm.ShowWindow(rememberForegroundWindow: false);
        _notifyIcon.BalloonTipTitle = "GeniaClipboard";
        _notifyIcon.BalloonTipText = "Программа работает в трее. Нажмите Ctrl+Shift+V, чтобы открыть историю.";

        _monitorWindow = new ClipboardMonitorWindow();
        _monitorWindow.ClipboardUpdated += (_, _) => _mainForm.HandleClipboardUpdate();
        _monitorWindow.HotKeyPressed += (_, _) => _mainForm.ShowWindow(rememberForegroundWindow: true);
        _monitorWindow.Start();
        _mainForm.SetMonitorStatus(
            _monitorWindow.ClipboardListenerRegistered,
            _monitorWindow.HotKeyRegistered);

        // Polling is a low-cost fallback for applications that update the
        // clipboard without delivering WM_CLIPBOARDUPDATE reliably.
        _clipboardPollTimer = new System.Windows.Forms.Timer
        {
            Interval = 700,
            Enabled = true
        };
        _clipboardPollTimer.Tick += (_, _) => _mainForm.PollClipboard();
        _mainForm.PollClipboard();
        _notifyIcon.ShowBalloonTip(2500);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clipboardPollTimer.Dispose();
            _monitorWindow.Dispose();
            _notifyIcon.Dispose();
            _mainForm.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ExitApplication()
    {
        if (_isExiting)
        {
            return;
        }

        _isExiting = true;
        _notifyIcon.Visible = false;
        _clipboardPollTimer.Stop();
        _monitorWindow.Dispose();
        _store.Flush();
        _settingsStore.Save();
        _mainForm.AllowClose();
        _mainForm.Close();
        ExitThread();
    }
}
