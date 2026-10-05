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

    public void SetMonitorStatus(bool clipboardListenerRegistered, bool hotKeyRegistered)
    {
        _clipboardListenerRegistered = clipboardListenerRegistered;
        _hotKeyRegistered = hotKeyRegistered;
        UpdateStatus();
    }

    public void HandleClipboardUpdate()
    {
        CaptureClipboardTextAsync();
    }

    public void PollClipboard()
    {
        CaptureClipboardTextAsync();
    }

    public void ShowWindow(bool rememberForegroundWindow)
    {
        if (rememberForegroundWindow)
        {
            RememberForegroundWindow();
        }

        _searchBox.Clear();
        RefreshHistoryList();
        ShowInTaskbar = true;
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
        BringToFront();
        _searchBox.Focus();
    }

    public void RememberForegroundWindow()
    {
        var foreground = NativeMethods.GetForegroundWindow();
        if (foreground != IntPtr.Zero && foreground != Handle)
        {
            _previousForegroundWindow = foreground;
        }
    }

    public void HideToTray()
    {
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

        var result = MessageBox.Show(
            "Удалить всю историю, включая закреплённые записи?",
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

    private static Button CreateButton(string text, bool classic = false)
    {
        var button = new Button
        {
            AutoSize = true,
            Height = 30,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 0, 6, 0),
            Padding = new Padding(8, 0, 8, 0),
            Font = new Font("Segoe UI", 8.75F),
            Cursor = Cursors.Hand
        };

        if (classic)
        {
            button.FlatStyle = FlatStyle.Standard;
            button.UseVisualStyleBackColor = true;
            button.BackColor = SystemColors.Control;
            button.ForeColor = SystemColors.ControlText;
        }
        else
        {
            button.BackColor = Color.White;
            button.ForeColor = Color.FromArgb(31, 41, 55);
            button.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(241, 245, 249);
        }

        return button;
    }
}
