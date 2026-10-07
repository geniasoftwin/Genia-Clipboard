namespace GeniaClipboard;

internal sealed class SettingsForm : Form
{
    private readonly VaultMode _currentVaultMode;
    private readonly ComboBox _vaultMode;
    private readonly TextBox _portablePassword;
    private readonly TextBox _portablePasswordConfirm;
    private readonly NumericUpDown _historyLimit;
    private readonly NumericUpDown _retentionDays;
    private readonly NumericUpDown _sensitiveExpire;
    private readonly NumericUpDown _clipboardClear;
    private readonly CheckBox _autoStart;
    private readonly CheckBox _autoJournal;
    private readonly CheckBox _privacyMarkers;
    private readonly CheckBox _detectSensitive;
    private readonly CheckBox _privateOnStart;
    private readonly TextBox _excludedProcesses;
    private readonly CheckBox _hotkeyCtrl;
    private readonly CheckBox _hotkeyAlt;
    private readonly CheckBox _hotkeyShift;
    private readonly CheckBox _hotkeyWin;
    private readonly ComboBox _hotkeyKey;

    public SettingsForm(AppSettings settings, VaultMode currentVaultMode, HotKeyDefinition currentHotKey)
    {
        _currentVaultMode = currentVaultMode;

        Text = "Настройки — GeniaClipboard";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(650, 700);
        MinimumSize = new Size(610, 620);
        Font = new Font("Segoe UI", 9F);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(14),
            ColumnCount = 2,
            RowCount = 0
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddSection(layout, "Хранилище");

        _vaultMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 340 };
        _vaultMode.Items.AddRange([
            "Windows Vault — без пароля, привязан к Windows-пользователю",
            "Portable Vault — мастер-пароль, перенос между ПК"
        ]);
        _vaultMode.SelectedIndex = currentVaultMode == VaultMode.Portable ? 1 : 0;
        AddRow(layout, "Режим vault", _vaultMode);

        _portablePassword = new TextBox { UseSystemPasswordChar = true, Width = 300 };
        _portablePasswordConfirm = new TextBox { UseSystemPasswordChar = true, Width = 300 };
        AddRow(layout, "Новый мастер-пароль", _portablePassword);
        AddRow(layout, "Повтор пароля", _portablePasswordConfirm);

        var passwordHint = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(360, 0),
            Text = "Оставьте поля пустыми, чтобы сохранить текущий пароль Portable Vault. При переходе с Windows Vault требуется новый пароль (минимум 10 символов).",
            ForeColor = Color.DimGray
        };
        AddRow(layout, "", passwordHint);

        _privateOnStart = new CheckBox
        {
            Text = "Запускать в Private Session (история только в памяти)",
            Checked = settings.PrivateSessionOnStart,
            AutoSize = true
        };
        AddRow(layout, "Private Session", _privateOnStart);

        AddSection(layout, "История и конфиденциальность");

        _historyLimit = Number(settings.HistoryLimit, 20, 100_000);
        AddRow(layout, "Лимит записей", _historyLimit);

        _retentionDays = Number(settings.RetentionDays, 0, 3650);
        AddRow(layout, "Удалять через, дней", _retentionDays);

        _privacyMarkers = new CheckBox
        {
            Text = "Уважать privacy-маркеры Windows clipboard",
            Checked = settings.RespectWindowsPrivacyMarkers,
            AutoSize = true
        };
        AddRow(layout, "Windows privacy", _privacyMarkers);

        _detectSensitive = new CheckBox
        {
            Text = "Распознавать токены, ключи и секреты",
            Checked = settings.DetectSensitiveText,
            AutoSize = true
        };
        AddRow(layout, "Sensitive detector", _detectSensitive);

        _sensitiveExpire = Number(settings.SensitiveExpireSeconds, 0, 86_400);
        AddRow(layout, "Sensitive auto-expire, сек.", _sensitiveExpire);

        _clipboardClear = Number(settings.ClipboardAutoClearSeconds, 0, 86_400);
        AddRow(layout, "Очистить clipboard через, сек.", _clipboardClear);

        _excludedProcesses = new TextBox
        {
            Multiline = true,
            Height = 70,
            ScrollBars = ScrollBars.Vertical,
            Text = string.Join("; ", settings.ExcludedProcesses)
        };
        AddRow(layout, "Не сохранять из процессов", _excludedProcesses);

        AddSection(layout, "Горячая клавиша");

        var hotkeyPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        _hotkeyCtrl = Modifier("Ctrl", currentHotKey, NativeMethods.ModControl);
        _hotkeyAlt = Modifier("Alt", currentHotKey, NativeMethods.ModAlt);
        _hotkeyShift = Modifier("Shift", currentHotKey, NativeMethods.ModShift);
        _hotkeyWin = Modifier("Win", currentHotKey, NativeMethods.ModWin);
        _hotkeyKey = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };

        var availableKeys = Enumerable.Range('A', 26)
            .Select(code => (Keys)code)
            .Concat(Enumerable.Range((int)Keys.F1, 12).Select(code => (Keys)code))
            .Concat([Keys.Space, Keys.Insert, Keys.Home, Keys.End])
            .Distinct()
            .ToList();

        foreach (var key in availableKeys)
        {
            _hotkeyKey.Items.Add(key);
        }

        var selectedKeyIndex = availableKeys.IndexOf(currentHotKey.Key);
        _hotkeyKey.SelectedIndex = selectedKeyIndex >= 0 ? selectedKeyIndex : availableKeys.IndexOf(Keys.V);

        hotkeyPanel.Controls.AddRange([_hotkeyCtrl, _hotkeyAlt, _hotkeyShift, _hotkeyWin, _hotkeyKey]);
        AddRow(layout, "Открыть историю", hotkeyPanel);

        AddSection(layout, "Система");

        _autoStart = new CheckBox
        {
            Text = "Запускать вместе с Windows (текущий пользователь)",
            Checked = settings.AutoStartEnabled,
            AutoSize = true
        };
        AddRow(layout, "Автозапуск", _autoStart);

        _autoJournal = new CheckBox
        {
            Text = "Вести TXT-журнал",
            Checked = settings.AutoJournalEnabled,
            AutoSize = true
        };
        AddRow(layout, "TXT-журнал", _autoJournal);

        var journalWarning = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(360, 0),
            Text = "Внимание: TXT-журнал остаётся незашифрованным. Sensitive-записи и Private Session в него не записываются.",
            ForeColor = Color.DarkRed
        };
        AddRow(layout, "", journalWarning);

        var saveButton = new Button
        {
            Text = "Сохранить",
            Width = 110,
            Height = 32
        };
        saveButton.Click += SaveButtonOnClick;

        var cancelButton = new Button
        {
            Text = "Отмена",
            Width = 90,
            Height = 32,
            DialogResult = DialogResult.Cancel
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(0, 10, 14, 8),
            FlowDirection = FlowDirection.RightToLeft
        };
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(cancelButton);

        Controls.Add(layout);
        Controls.Add(buttons);
        CancelButton = cancelButton;
    }

    public VaultMode RequestedVaultMode => _vaultMode.SelectedIndex == 1
        ? VaultMode.Portable
        : VaultMode.Windows;

    public string? NewPortablePassword =>
        string.IsNullOrEmpty(_portablePassword.Text) ? null : _portablePassword.Text;

    public int HistoryLimit => decimal.ToInt32(_historyLimit.Value);

    public int RetentionDays => decimal.ToInt32(_retentionDays.Value);

    public int SensitiveExpireSeconds => decimal.ToInt32(_sensitiveExpire.Value);

    public int ClipboardAutoClearSeconds => decimal.ToInt32(_clipboardClear.Value);

    public bool AutoStartEnabled => _autoStart.Checked;

    public bool AutoJournalEnabled => _autoJournal.Checked;

    public bool RespectWindowsPrivacyMarkers => _privacyMarkers.Checked;

    public bool DetectSensitiveText => _detectSensitive.Checked;

    public bool PrivateSessionOnStart => _privateOnStart.Checked;

    public IReadOnlyList<string> ExcludedProcesses =>
        _excludedProcesses.Text
            .Split([';', ',', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

    public HotKeyDefinition HotKey
    {
        get
        {
            uint modifiers = 0;
            if (_hotkeyCtrl.Checked) modifiers |= NativeMethods.ModControl;
            if (_hotkeyAlt.Checked) modifiers |= NativeMethods.ModAlt;
            if (_hotkeyShift.Checked) modifiers |= NativeMethods.ModShift;
            if (_hotkeyWin.Checked) modifiers |= NativeMethods.ModWin;

            return new HotKeyDefinition(modifiers, (Keys)(_hotkeyKey.SelectedItem ?? Keys.V));
        }
    }

    private void SaveButtonOnClick(object? sender, EventArgs e)
    {
        if (HotKey.Modifiers == 0)
        {
            MessageBox.Show(this, "Для глобального хоткея выберите хотя бы один модификатор.", "GeniaClipboard");
            return;
        }

        var passwordWasEntered = !string.IsNullOrEmpty(_portablePassword.Text);
        var passwordRequired = RequestedVaultMode == VaultMode.Portable &&
                               _currentVaultMode != VaultMode.Portable;

        if (passwordRequired && !passwordWasEntered)
        {
            MessageBox.Show(this, "Для перехода в Portable Vault задайте мастер-пароль.", "GeniaClipboard");
            return;
        }

        if (passwordWasEntered)
        {
            if (_portablePassword.Text.Length < 10)
            {
                MessageBox.Show(this, "Мастер-пароль должен содержать минимум 10 символов.", "GeniaClipboard");
                return;
            }

            if (!string.Equals(_portablePassword.Text, _portablePasswordConfirm.Text, StringComparison.Ordinal))
            {
                MessageBox.Show(this, "Мастер-пароли не совпадают.", "GeniaClipboard");
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private static NumericUpDown Number(int value, int minimum, int maximum)
    {
        return new NumericUpDown
        {
            Minimum = minimum,
            Maximum = maximum,
            Value = Math.Clamp(value, minimum, maximum),
            ThousandsSeparator = true,
            Width = 140
        };
    }

    private static CheckBox Modifier(string text, HotKeyDefinition hotKey, uint flag)
    {
        return new CheckBox
        {
            Text = text,
            AutoSize = true,
            Checked = (hotKey.Modifiers & flag) != 0,
            Margin = new Padding(0, 4, 10, 0)
        };
    }

    private static void AddSection(TableLayoutPanel layout, string title)
    {
        var label = new Label
        {
            Text = title,
            Font = new Font("Segoe UI Semibold", 10.5F),
            AutoSize = true,
            Padding = new Padding(0, 12, 0, 5)
        };

        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(label, 0, row);
        layout.SetColumnSpan(label, 2);
    }

    private static void AddRow(TableLayoutPanel layout, string caption, Control control)
    {
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = caption,
            AutoSize = true,
            Padding = new Padding(0, 7, 8, 7),
            ForeColor = Color.FromArgb(55, 65, 81)
        };

        control.Margin = new Padding(0, 4, 0, 4);
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }
}
