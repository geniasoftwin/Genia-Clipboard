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
    private readonly ToolStripMenuItem _privateSessionMenuItem;
    private readonly ToolStripMenuItem _autoJournalMenuItem;
    private bool _isExiting;

    public GeniaClipboardApplicationContext(
        AppSettingsStore settingsStore,
        HistoryStore store)
    {
        _store = store;
        _settingsStore = settingsStore;
        _journal = new TextJournalService
        {
            Enabled = _settingsStore.Settings.AutoJournalEnabled
        };
        _mainForm = new MainForm(_store, _journal, _settingsStore.Settings);
        _mainForm.SettingsRequested += (_, _) => OpenSettings();

        var openMenuItem = new ToolStripMenuItem(UiText.T("Открыть"));
        openMenuItem.Click += (_, _) => _mainForm.ShowWindow();

        _captureMenuItem = new ToolStripMenuItem(UiText.T("Сохранять скопированное"))
        {
            Checked = true,
            CheckOnClick = true
        };
        _captureMenuItem.CheckedChanged += (_, _) =>
            _mainForm.SetCaptureEnabled(_captureMenuItem.Checked);

        _privateSessionMenuItem = new ToolStripMenuItem(UiText.T("Private Session — только память"))
        {
            Checked = _store.IsPrivateSession
        };
        _privateSessionMenuItem.Click += (_, _) => TogglePrivateSession();

        _autoJournalMenuItem = new ToolStripMenuItem(UiText.T("Автожурнал TXT"))
        {
            Checked = _journal.Enabled,
            CheckOnClick = true
        };
        _autoJournalMenuItem.CheckedChanged += (_, _) =>
        {
            _mainForm.SetAutoJournalEnabled(_autoJournalMenuItem.Checked);
            _settingsStore.Settings.AutoJournalEnabled = _autoJournalMenuItem.Checked;
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

        var settingsMenuItem = new ToolStripMenuItem(UiText.T("Настройки…"));
        settingsMenuItem.Click += (_, _) => OpenSettings();

        var aboutMenuItem = new ToolStripMenuItem(UiText.T("О программе"));
        aboutMenuItem.Click += (_, _) =>
        {
            using var about = new AboutForm();
            about.ShowDialog(_mainForm);
        };

        var clearMenuItem = new ToolStripMenuItem(UiText.T("Очистить историю"));
        clearMenuItem.Click += (_, _) => _mainForm.ClearHistoryWithConfirmation();

        var exitMenuItem = new ToolStripMenuItem(UiText.T("Выход"));
        exitMenuItem.Click += (_, _) => ExitApplication();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange([
            openMenuItem,
            settingsMenuItem,
            aboutMenuItem,
            new ToolStripSeparator(),
            _captureMenuItem,
            _privateSessionMenuItem,
            _autoJournalMenuItem,
            clearMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
        ]);

        _monitorWindow = new ClipboardMonitorWindow();
        _monitorWindow.ClipboardUpdated += (_, _) => _mainForm.HandleClipboardUpdate();
        _monitorWindow.HotKeyPressed += (_, _) => _mainForm.ShowWindow();

        var configuredHotKey = HotKeyDefinition.FromSettings(_settingsStore.Settings);
        _monitorWindow.Start(configuredHotKey);

        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Icon = _mainForm.Icon,
            Text = BuildTrayText(_monitorWindow.CurrentHotKey),
            ContextMenuStrip = menu
        };
        _notifyIcon.DoubleClick += (_, _) => _mainForm.ShowWindow();
        _notifyIcon.BalloonTipTitle = "GeniaClipboard 0.5.7";
        _notifyIcon.BalloonTipText = UiText.T("Clipboard Firewall работает локально. История хранится в зашифрованном vault.");

        _mainForm.SetMonitorStatus(
            _monitorWindow.ClipboardListenerRegistered,
            _monitorWindow.HotKeyRegistered,
            _monitorWindow.CurrentHotKey);

        if (_settingsStore.Settings.AutoStartEnabled &&
            !AutoStartService.SetEnabled(true, out _))
        {
            _settingsStore.Settings.AutoStartEnabled = false;
            _settingsStore.Save();
        }

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

    private void OpenSettings()
    {
        using var dialog = new SettingsForm(
            _settingsStore.Settings,
            _store.VaultMode,
            _monitorWindow.CurrentHotKey);

        if (dialog.ShowDialog(_mainForm) != DialogResult.OK)
        {
            return;
        }

        var oldLanguage = _settingsStore.Settings.Language;
        var oldHotKey = _monitorWindow.CurrentHotKey;
        var oldAutoStart = _settingsStore.Settings.AutoStartEnabled;

        if (!_monitorWindow.TrySetHotKey(dialog.HotKey))
        {
            MessageBox.Show(
                _mainForm,
                UiText.IsEnglish ? $"Hotkey {dialog.HotKey.ToDisplayString()} is already in use by another application." : $"Хоткей {dialog.HotKey.ToDisplayString()} уже занят другой программой.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (!AutoStartService.SetEnabled(dialog.AutoStartEnabled, out var autoStartError))
        {
            _monitorWindow.TrySetHotKey(oldHotKey);
            MessageBox.Show(
                _mainForm,
                autoStartError ?? UiText.T("Не удалось изменить автозапуск."),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var vaultNeedsChange =
            dialog.RequestedVaultMode != _store.VaultMode ||
            (dialog.RequestedVaultMode == VaultMode.Portable &&
             dialog.NewPortablePassword is not null);

        if (vaultNeedsChange &&
            !_store.ChangeVaultMode(dialog.RequestedVaultMode, dialog.NewPortablePassword))
        {
            _monitorWindow.TrySetHotKey(oldHotKey);
            AutoStartService.SetEnabled(oldAutoStart, out _);
            MessageBox.Show(
                _mainForm,
                _store.LastError is null ? UiText.T("Не удалось изменить режим vault.") : UiText.Error(_store.LastError),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var settings = _settingsStore.Settings;
        settings.HotKeyModifiers = dialog.HotKey.Modifiers;
        settings.HotKeyKey = (int)dialog.HotKey.Key;
        settings.AutoStartEnabled = dialog.AutoStartEnabled;
        settings.AutoJournalEnabled = dialog.AutoJournalEnabled;
        settings.HistoryLimit = dialog.HistoryLimit;
        settings.RetentionDays = dialog.RetentionDays;
        settings.RespectWindowsPrivacyMarkers = dialog.RespectWindowsPrivacyMarkers;
        settings.DetectSensitiveText = dialog.DetectSensitiveText;
        settings.SensitiveExpireSeconds = dialog.SensitiveExpireSeconds;
        settings.ClipboardAutoClearSeconds = dialog.ClipboardAutoClearSeconds;
        settings.PrivateSessionOnStart = dialog.PrivateSessionOnStart;
        settings.PreferredVaultMode = dialog.RequestedVaultMode;
        settings.Language = dialog.SelectedLanguage;
        settings.ExcludedProcesses = dialog.ExcludedProcesses.ToList();
        settings.Normalize();

        _store.ApplyPolicy(settings.HistoryLimit, settings.RetentionDays);
        _journal.Enabled = settings.AutoJournalEnabled;
        _autoJournalMenuItem.Checked = settings.AutoJournalEnabled;
        _settingsStore.Save();

        _notifyIcon.Text = BuildTrayText(_monitorWindow.CurrentHotKey);
        _mainForm.SetMonitorStatus(
            _monitorWindow.ClipboardListenerRegistered,
            _monitorWindow.HotKeyRegistered,
            _monitorWindow.CurrentHotKey);
        _mainForm.RefreshFromStore();

        if (_settingsStore.LastError is not null)
        {
            MessageBox.Show(
                _mainForm,
                _settingsStore.LastError,
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        if (settings.Language != oldLanguage && _settingsStore.LastError is null)
        {
            MessageBox.Show(
                _mainForm,
                UiText.T("Язык интерфейса изменится после перезапуска GeniaClipboard. Для сохранения зашифрованной истории выйдите через трей и запустите приложение снова."),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    private void TogglePrivateSession()
    {
        if (!_store.IsPrivateSession)
        {
            var result = MessageBox.Show(
                _mainForm,
                UiText.T("В Private Session постоянная зашифрованная история будет временно скрыта. Новые записи останутся только в памяти и исчезнут при выходе. Продолжить?"),
                "GeniaClipboard — Private Session",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _store.BeginPrivateSession();
            _privateSessionMenuItem.Checked = true;
            _mainForm.RefreshFromStore();
            return;
        }

        var exitResult = MessageBox.Show(
            _mainForm,
            UiText.T("Завершить Private Session? Вся временная история этой сессии будет удалена без сохранения."),
            "GeniaClipboard — Private Session",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (exitResult != DialogResult.Yes)
        {
            return;
        }

        if (!_store.EndPrivateSession())
        {
            MessageBox.Show(
                _mainForm,
                _store.LastError is null ? UiText.T("Не удалось открыть постоянную историю.") : UiText.Error(_store.LastError),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        _privateSessionMenuItem.Checked = false;
        _mainForm.RefreshFromStore();
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

    private static string BuildTrayText(HotKeyDefinition hotKey)
    {
        var text = $"GeniaClipboard — {hotKey.ToDisplayString()}";
        return text.Length <= 127 ? text : text[..127];
    }
}
