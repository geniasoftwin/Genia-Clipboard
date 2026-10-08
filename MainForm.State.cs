namespace GeniaClipboard;

internal sealed partial class MainForm
{
    public bool CaptureEnabled { get; private set; } = true;

    public void SetCaptureEnabled(bool enabled)
    {
        CaptureEnabled = enabled;
        UpdateStatus();
    }

    public void SetAutoJournalEnabled(bool enabled)
    {
        _journal.Enabled = enabled;
        UpdateStatus();
    }

    public void SetMonitorStatus(
        bool clipboardListenerRegistered,
        bool hotKeyRegistered,
        HotKeyDefinition currentHotKey)
    {
        _clipboardListenerRegistered = clipboardListenerRegistered;
        _hotKeyRegistered = hotKeyRegistered;
        SetHotKeyDisplay(currentHotKey);
        UpdateStatus();
    }

    public void SetHotKeyDisplay(HotKeyDefinition hotKey)
    {
        _hotKeyLabel.Text = $"Firewall ON · {hotKey.ToDisplayString()}";
    }

    public void HandleClipboardUpdate()
    {
        CaptureClipboardTextAsync();
    }

    public void PollClipboard()
    {
        if (_store.RemoveExpired() > 0)
        {
            RefreshHistoryList();
        }

        CaptureClipboardTextAsync();
    }

    public void RefreshFromStore()
    {
        RefreshHistoryList();
    }

    public void ShowWindow(bool rememberForegroundWindow)
    {
        // Only the global shortcut explicitly authorizes returning to an
        // external window. Opening the UI from the tray never inherits a
        // target captured by an earlier, unrelated interaction.
        _openingFromShortcut = rememberForegroundWindow;

        if (rememberForegroundWindow)
        {
            RememberForegroundWindow();
        }
        else
        {
            ClearPasteTarget();
        }

        try
        {
            _searchBox.Clear();
            RefreshHistoryList();
            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            BringToFront();
            _searchBox.Focus();
        }
        finally
        {
            _openingFromShortcut = false;
        }
    }

    private void RememberForegroundWindow()
    {
        ClearPasteTarget();
        var foreground = NativeMethods.GetForegroundWindow();

        if (foreground == IntPtr.Zero ||
            foreground == Handle ||
            !NativeMethods.IsWindow(foreground) ||
            NativeMethods.GetWindowThreadProcessId(foreground, out var processId) == 0 ||
            processId == 0 ||
            processId == (uint)Environment.ProcessId)
        {
            return;
        }

        _previousForegroundWindow = foreground;
        _pasteTargetProcessId = processId;
    }

    private void ClearPasteTarget()
    {
        _previousForegroundWindow = IntPtr.Zero;
        _pasteTargetProcessId = 0;
    }

    private bool IsValidPasteTarget(IntPtr target)
    {
        return target != IntPtr.Zero &&
               target != Handle &&
               NativeMethods.IsWindow(target) &&
               NativeMethods.GetWindowThreadProcessId(target, out var processId) != 0 &&
               processId != 0 &&
               processId != (uint)Environment.ProcessId &&
               processId == _pasteTargetProcessId;
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        // Re-activating GeniaClipboard manually (taskbar, Alt+Tab, mouse)
        // cancels an old shortcut target. Never paste into a stale window.
        if (!_openingFromShortcut)
        {
            ClearPasteTarget();
        }
    }

    public void HideToTray()
    {
        ClearPasteTarget();
        Hide();
        ShowInTaskbar = false;
    }

    public void AllowClose() => _allowClose = true;

    public void ClearHistoryWithConfirmation()
    {
        if (_store.Items.Count == 0)
        {
            return;
        }

        var message = _store.IsPrivateSession
            ? "Удалить всю временную историю Private Session?"
            : "Удалить всю зашифрованную историю, включая закреплённые записи?";

        var result = MessageBox.Show(
            message,
            "GeniaClipboard",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        _store.Clear();
        RefreshHistoryList();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_allowClose && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }

        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clipboardClearTimer.Dispose();
            _toolTip.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ScheduleClipboardClear(uint sequenceNumber)
    {
        _clipboardClearTimer.Stop();

        if (_settings.ClipboardAutoClearSeconds <= 0 || sequenceNumber == 0)
        {
            return;
        }

        _clipboardSequenceToClear = sequenceNumber;
        var milliseconds = (long)_settings.ClipboardAutoClearSeconds * 1000L;
        _clipboardClearTimer.Interval = (int)Math.Clamp(milliseconds, 1000L, int.MaxValue);
        _clipboardClearTimer.Start();
    }

    private void ClipboardClearTimerOnTick(object? sender, EventArgs e)
    {
        _clipboardClearTimer.Stop();

        try
        {
            var currentSequence = NativeMethods.GetClipboardSequenceNumber();
            if (currentSequence == 0 || currentSequence != _clipboardSequenceToClear)
            {
                return;
            }

            Clipboard.Clear();
            _lastClipboardSequenceNumber = NativeMethods.GetClipboardSequenceNumber();
        }
        catch
        {
            // Clipboard ownership is transient. A failed cleanup is not fatal.
        }
    }

    private enum ButtonTone
    {
        Neutral,
        Primary,
        Danger
    }

    private static Button CreateButton(string text, ButtonTone tone = ButtonTone.Neutral)
    {
        var button = new Button
        {
            AutoSize = true,
            Height = 30,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            Margin = new Padding(0, 0, 6, 0),
            Padding = new Padding(8, 0, 8, 0),
            Font = new Font("Segoe UI", 8.75F),
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 1;
        button.EnabledChanged += (_, _) => ApplyButtonTone(button, tone);
        ApplyButtonTone(button, tone);
        return button;
    }

    private static void ApplyButtonTone(Button button, ButtonTone tone)
    {
        if (!button.Enabled)
        {
            button.BackColor = Color.FromArgb(241, 245, 249);
            button.ForeColor = Color.FromArgb(148, 163, 184);
            button.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            return;
        }

        switch (tone)
        {
            case ButtonTone.Primary:
                button.BackColor = Color.FromArgb(37, 99, 235);
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
                break;

            case ButtonTone.Danger:
                button.BackColor = Color.FromArgb(185, 28, 28);
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = Color.FromArgb(185, 28, 28);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(153, 27, 27);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(127, 29, 29);
                break;

            default:
                button.BackColor = Color.White;
                button.ForeColor = Color.FromArgb(31, 41, 55);
                button.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(241, 245, 249);
                break;
        }
    }
}
