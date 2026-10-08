namespace GeniaClipboard;

/// <summary>
/// Four focused settings pages. Each page scrolls independently while the
/// Save/Cancel footer remains visible, including at increased Windows DPI.
/// </summary>
internal sealed class SettingsForm : Form
{
    private readonly VaultMode _currentVaultMode;
    private readonly ComboBox _vaultMode;
    private readonly TextBox _portablePassword;
    private readonly TextBox _portablePasswordConfirm;
    private readonly Label _vaultHint;
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
    private readonly TabControl _tabs;

    public SettingsForm(AppSettings settings, VaultMode currentVaultMode, HotKeyDefinition currentHotKey)
    {
        _currentVaultMode = currentVaultMode;

        Text = "Настройки — GeniaClipboard";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 540);
        MinimumSize = new Size(650, 450);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.White;

        _tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(16, 8),
            Margin = new Padding(0)
        };

        var securityPage = CreatePage("Безопасность", out var security);
        var historyPage = CreatePage("История", out var history);
        var hotkeyPage = CreatePage("Горячие клавиши", out var hotkeys);
        var systemPage = CreatePage("Система", out var system);
        _tabs.TabPages.AddRange([securityPage, historyPage, hotkeyPage, systemPage]);

        // Security: vault and secret-handling controls.
        AddSection(security, "Зашифрованное хранилище");

        _vaultMode = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DropDownWidth = 390
        };
        _vaultMode.Items.AddRange([
            "Windows Vault (DPAPI)",
            "Portable Vault (мастер-пароль)"
        ]);
        _vaultMode.SelectedIndex = currentVaultMode == VaultMode.Portable ? 1 : 0;
        AddRow(security, "Режим Vault", _vaultMode);

        _vaultHint = AddNote(
            security,
            "Windows Vault привязан к текущей учётной записи Windows. " +
            "Portable Vault можно переносить между компьютерами вместе с зашифрованной историей.");

        _portablePassword = new TextBox { UseSystemPasswordChar = true };
        _portablePasswordConfirm = new TextBox { UseSystemPasswordChar = true };
        AddRow(security, "Новый мастер-пароль", _portablePassword);
        AddRow(security, "Повтор пароля", _portablePasswordConfirm);
        AddNote(
            security,
            "Чтобы оставить существующий пароль Portable Vault, не заполняйте поля. " +
            "При переходе в Portable Vault задайте новый пароль минимум из 10 символов.");

        _privateOnStart = new CheckBox
        {
            Text = "Запускать в Private Session",
            Checked = settings.PrivateSessionOnStart,
            AutoSize = true
        };
        AddRow(security, "Приватный запуск", _privateOnStart);
        AddNote(
            security,
            "Private Session сохраняет временные записи только в памяти и не записывает их в TXT-журнал.");

        AddSection(security, "Защита конфиденциальных данных");

        _privacyMarkers = new CheckBox
        {
            Text = "Уважать privacy-маркеры Windows",
            Checked = settings.RespectWindowsPrivacyMarkers,
            AutoSize = true
        };
        AddRow(security, "Маркеры Windows", _privacyMarkers);

        _detectSensitive = new CheckBox
        {
            Text = "Распознавать токены и секреты",
            Checked = settings.DetectSensitiveText,
            AutoSize = true
        };
        AddRow(security, "Sensitive detector", _detectSensitive);

        _sensitiveExpire = Number(settings.SensitiveExpireSeconds, 0, 86_400);
        AddRow(security, "Удаление секретов, сек.", _sensitiveExpire);
        AddNote(
            security,
            "0 = не удалять автоматически. Распознавание секретов эвристическое " +
            "и не гарантирует обнаружение каждого пароля или токена.");

        // History: limits, clipboard cleanup and process exclusions.
        AddSection(history, "Хранение истории");

        _historyLimit = Number(settings.HistoryLimit, 20, 100_000);
        AddRow(history, "Лимит записей", _historyLimit);

        _retentionDays = Number(settings.RetentionDays, 0, 3650);
        AddRow(history, "Удалять через, дней", _retentionDays);
        AddNote(
            history,
            "0 = хранить без ограничения по времени. Закреплённые записи " +
            "не удаляются по общему лимиту и сроку хранения.");

        AddSection(history, "Системный буфер обмена");

        _clipboardClear = Number(settings.ClipboardAutoClearSeconds, 0, 86_400);
        AddRow(history, "Автоочистка, сек.", _clipboardClear);
        AddNote(
            history,
            "0 = выключено. Автоочистка удалит содержимое буфера, только если оно " +
            "не изменилось с момента копирования.");

        AddSection(history, "Исключения приложений");

        AddNote(
            history,
            "Не сохранять содержимое буфера из указанных процессов. " +
            "Введите имена процессов без .exe, по одному на строку; " +
            "например: Bitwarden или KeePassXC.");

        _excludedProcesses = new TextBox
        {
            Multiline = true,
            AcceptsReturn = true,
            ScrollBars = ScrollBars.Vertical,
            MinimumSize = new Size(0, 114),
            Height = 114,
            Text = string.Join(Environment.NewLine, settings.ExcludedProcesses),
            PlaceholderText = "Bitwarden" + Environment.NewLine + "KeePassXC"
        };
        AddFullWidthControl(history, _excludedProcesses);
        AddNote(
            history,
            "Имя процесса помогает фильтровать приложения, но не является " +
            "надёжным подтверждением происхождения данных.");

        // Hotkey: separate page for an uncluttered combination picker.
        AddSection(hotkeys, "Открытие истории");

        _hotkeyCtrl = Modifier("Ctrl", currentHotKey, NativeMethods.ModControl);
        _hotkeyAlt = Modifier("Alt", currentHotKey, NativeMethods.ModAlt);
        _hotkeyShift = Modifier("Shift", currentHotKey, NativeMethods.ModShift);
        _hotkeyWin = Modifier("Win", currentHotKey, NativeMethods.ModWin);

        _hotkeyKey = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 95
        };

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
        _hotkeyKey.SelectedIndex = selectedKeyIndex >= 0
            ? selectedKeyIndex
            : availableKeys.IndexOf(Keys.V);

        var hotkeyPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            Margin = new Padding(0)
        };
        hotkeyPanel.Controls.AddRange([
            _hotkeyCtrl, _hotkeyAlt, _hotkeyShift, _hotkeyWin, _hotkeyKey
        ]);
        AddFullWidthControl(hotkeys, hotkeyPanel);

        AddNote(
            hotkeys,
            "Для глобального сочетания выберите хотя бы один модификатор. " +
            "Если комбинация уже занята другим приложением, текущая будет сохранена.");

        // System: only optional Windows integrations and plaintext journal.
        AddSection(system, "Запуск программы");

        _autoStart = new CheckBox
        {
            Text = "Запускать вместе с Windows",
            Checked = settings.AutoStartEnabled,
            AutoSize = true
        };
        AddRow(system, "Автозапуск", _autoStart);
        AddNote(
            system,
            "Используется автозапуск только для текущего пользователя Windows. " +
            "Права администратора не требуются.");

        AddSection(system, "Экспорт и журнал");

        _autoJournal = new CheckBox
        {
            Text = "Вести ежедневный TXT-журнал",
            Checked = settings.AutoJournalEnabled,
            AutoSize = true
        };
        AddRow(system, "TXT-журнал", _autoJournal);
        AddNote(
            system,
            "Внимание: TXT-журнал не шифруется. Sensitive-записи и записи " +
            "Private Session в него не добавляются.",
            Color.DarkRed);

        _vaultMode.SelectedIndexChanged += (_, _) => UpdateVaultFields();
        UpdateVaultFields();

        var saveButton = new Button
        {
            Text = "Сохранить",
            Width = 114,
            Height = 33
        };
        saveButton.Click += SaveButtonOnClick;

        var cancelButton = new Button
        {
            Text = "Отмена",
            Width = 100,
            Height = 33,
            DialogResult = DialogResult.Cancel
        };

        var footerButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Width = 245,
            Padding = new Padding(0, 10, 14, 0)
        };
        footerButtons.Controls.Add(saveButton);
        footerButtons.Controls.Add(cancelButton);

        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            BackColor = Color.FromArgb(248, 250, 252)
        };
        footer.Controls.Add(footerButtons);
        footer.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(222, 226, 231));
            e.Graphics.DrawLine(pen, 0, 0, footer.ClientSize.Width, 0);
        };

        Controls.Add(_tabs);
        Controls.Add(footer);
        CancelButton = cancelButton;
    }

    public VaultMode RequestedVaultMode => _vaultMode.SelectedIndex == 1
        ? VaultMode.Portable
        : VaultMode.Windows;

    public string? NewPortablePassword =>
        RequestedVaultMode == VaultMode.Portable && !string.IsNullOrEmpty(_portablePassword.Text)
            ? _portablePassword.Text
            : null;

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
            _tabs.SelectedIndex = 2;
            MessageBox.Show(
                this,
                "Для глобального хоткея выберите хотя бы один модификатор.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var passwordWasEntered = NewPortablePassword is not null;
        var passwordRequired = RequestedVaultMode == VaultMode.Portable &&
                               _currentVaultMode != VaultMode.Portable;

        if (passwordRequired && !passwordWasEntered)
        {
            _tabs.SelectedIndex = 0;
            MessageBox.Show(
                this,
                "Для перехода в Portable Vault задайте мастер-пароль.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            _portablePassword.Focus();
            return;
        }

        if (passwordWasEntered)
        {
            if (_portablePassword.Text.Length < 10)
            {
                _tabs.SelectedIndex = 0;
                MessageBox.Show(
                    this,
                    "Мастер-пароль должен содержать минимум 10 символов.",
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _portablePassword.Focus();
                return;
            }

            if (!string.Equals(_portablePassword.Text, _portablePasswordConfirm.Text, StringComparison.Ordinal))
            {
                _tabs.SelectedIndex = 0;
                MessageBox.Show(
                    this,
                    "Мастер-пароли не совпадают.",
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _portablePasswordConfirm.Focus();
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateVaultFields()
    {
        var portable = RequestedVaultMode == VaultMode.Portable;
        _portablePassword.Enabled = portable;
        _portablePasswordConfirm.Enabled = portable;

        if (!portable)
        {
            _portablePassword.Clear();
            _portablePasswordConfirm.Clear();
        }

        _vaultHint.Text = portable
            ? "Portable Vault переносится между компьютерами. Не потеряйте мастер-пароль: без него историю нельзя восстановить."
            : "Windows Vault использует DPAPI текущей учётной записи Windows и не требует мастер-пароля.";
    }

    private static TabPage CreatePage(string title, out TableLayoutPanel layout)
    {
        var page = new TabPage(title)
        {
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(0),
            UseVisualStyleBackColor = false
        };

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            GrowStyle = TableLayoutPanelGrowStyle.AddRows,
            ColumnCount = 2,
            Padding = new Padding(18, 12, 18, 22),
            Margin = new Padding(0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 188));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        page.Controls.Add(layout);
        return page;
    }

    private static void AddSection(TableLayoutPanel layout, string title)
    {
        var heading = new Label
        {
            Text = title,
            Font = new Font("Segoe UI Semibold", 10.5F),
            ForeColor = Color.FromArgb(31, 41, 55),
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 9)
        };
        AddFullWidthControl(layout, heading);
    }

    private static Label AddNote(TableLayoutPanel layout, string message, Color? color = null)
    {
        var note = new Label
        {
            Text = message,
            AutoSize = true,
            MaximumSize = new Size(570, 0),
            ForeColor = color ?? Color.FromArgb(96, 106, 120),
            Margin = new Padding(0, 4, 0, 12)
        };
        AddFullWidthControl(layout, note);
        return note;
    }

    private static void AddRow(TableLayoutPanel layout, string caption, Control control)
    {
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = caption,
            AutoSize = true,
            MaximumSize = new Size(178, 0),
            ForeColor = Color.FromArgb(55, 65, 81),
            Margin = new Padding(0, 9, 10, 9)
        };

        if (control is TextBox or ComboBox or CheckBox)
        {
            control.Dock = DockStyle.Top;
        }
        control.Margin = new Padding(0, 5, 0, 7);

        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private static void AddFullWidthControl(TableLayoutPanel layout, Control control)
    {
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        if (control is TextBox or FlowLayoutPanel)
        {
            control.Dock = DockStyle.Top;
        }
        layout.Controls.Add(control, 0, row);
        layout.SetColumnSpan(control, 2);
    }

    private static NumericUpDown Number(int value, int minimum, int maximum)
    {
        return new NumericUpDown
        {
            Minimum = minimum,
            Maximum = maximum,
            Value = Math.Clamp(value, minimum, maximum),
            ThousandsSeparator = true,
            Width = 155
        };
    }

    private static CheckBox Modifier(string text, HotKeyDefinition hotKey, uint flag)
    {
        return new CheckBox
        {
            Text = text,
            AutoSize = true,
            Checked = (hotKey.Modifiers & flag) != 0,
            Margin = new Padding(0, 4, 16, 4)
        };
    }
}
