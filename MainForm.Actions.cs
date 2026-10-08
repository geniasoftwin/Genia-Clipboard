using System.Runtime.InteropServices;

namespace GeniaClipboard;

internal sealed partial class MainForm
{
    private async Task CopySelectedAsync()
    {
        var selected = SelectedEntries;
        if (selected.Count == 0)
        {
            return;
        }

        if (!TryBuildSelectedText(out var text))
        {
            return;
        }

        if (await TrySetClipboardTextAsync(text))
        {
            _statusLabel.Text = selected.Count == 1
                ? "Скопировано"
                : $"Скопировано элементов: {selected.Count}";
        }
    }

    private async Task PasteSelectedAsync()
    {
        if (SelectedEntries.Count == 0)
        {
            return;
        }

        var targetWindow = _previousForegroundWindow;

        // A paste target belongs to a deliberate global-hotkey invocation.
        // It must never be reused after the user manually returns to the app.
        if (!IsValidPasteTarget(targetWindow))
        {
            ClearPasteTarget();
            MessageBox.Show(
                this,
                "Чтобы вставить текст автоматически, поставьте курсор в нужном приложении " +
                "и откройте GeniaClipboard глобальной горячей клавишей.\n\n" +
                "Если окно истории открыто вручную, используйте «Копировать», " +
                "а затем Ctrl+V в нужном приложении.",
                "GeniaClipboard — Вставить",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (!TryBuildSelectedText(out var text) ||
            !await TrySetClipboardTextAsync(text))
        {
            return;
        }

        // Focus changes are never assumed: Windows can refuse foreground
        // activation. The target is validated immediately before Ctrl+V.
        _ = NativeMethods.SetForegroundWindow(targetWindow);
        await Task.Delay(110);

        if (!IsValidPasteTarget(targetWindow) ||
            NativeMethods.GetForegroundWindow() != targetWindow)
        {
            ClearPasteTarget();
            MessageBox.Show(
                this,
                "Windows не разрешила переключиться в целевое окно. " +
                "Текст уже скопирован: перейдите в нужное приложение и нажмите Ctrl+V.",
                "GeniaClipboard — Вставить",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        HideToTray();
        await Task.Delay(60);

        if (NativeMethods.IsWindow(targetWindow) &&
            NativeMethods.GetForegroundWindow() == targetWindow)
        {
            NativeMethods.SendCtrlV();
        }
    }

    private async Task<bool> TrySetClipboardTextAsync(string text)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                var data = new DataObject();
                data.SetData(DataFormats.UnicodeText, true, text);
                data.SetData(InternalClipboardFormat, false, "1");

                using var excludeStream = new MemoryStream(BitConverter.GetBytes(1));
                using var historyStream = new MemoryStream(BitConverter.GetBytes(0));
                using var cloudStream = new MemoryStream(BitConverter.GetBytes(0));
                data.SetData("ExcludeClipboardContentFromMonitorProcessing", false, excludeStream);
                data.SetData("CanIncludeInClipboardHistory", false, historyStream);
                data.SetData("CanUploadToCloudClipboard", false, cloudStream);

                Clipboard.SetDataObject(data, copy: true);
                _lastClipboardSequenceNumber = NativeMethods.GetClipboardSequenceNumber();
                ScheduleClipboardClear(_lastClipboardSequenceNumber);
                return true;
            }
            catch (ExternalException)
            {
                await Task.Delay(40 * (attempt + 1));
            }
        }

        MessageBox.Show(
            "Буфер обмена сейчас занят другой программой. Попробуйте ещё раз.",
            "GeniaClipboard",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        return false;
    }

    private void EditSelectedEntry()
    {
        var entry = SelectedEntry;
        if (entry is null)
        {
            return;
        }

        using var dialog = new EditEntryForm(entry.Text);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var edited = dialog.EditedText;
        if (string.Equals(edited, entry.Text, StringComparison.Ordinal))
        {
            return;
        }

        if (_settings.DetectSensitiveText && SensitiveDataDetector.LooksSensitive(edited))
        {
            entry.IsSensitive = true;
            if (!entry.IsPinned && _settings.SensitiveExpireSeconds > 0)
            {
                entry.ExpiresAt = DateTimeOffset.Now.AddSeconds(_settings.SensitiveExpireSeconds);
            }
        }

        if (!_store.UpdateText(entry, edited))
        {
            MessageBox.Show(
                this,
                "Не удалось сохранить изменённую запись.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        RefreshHistoryList();
    }

    private void TogglePinned()
    {
        var entry = SelectedEntry;
        if (entry is null)
        {
            return;
        }

        _store.TogglePinned(entry);
        RefreshHistoryList();
    }

    private void DeleteSelected()
    {
        var selected = SelectedEntries.ToList();
        if (selected.Count == 0)
        {
            return;
        }

        _store.RemoveMany(selected);
        RefreshHistoryList();
    }

    private void ExportToTxt()
    {
        var selected = SelectedEntries;
        var entries = selected.Count >= 2
            ? selected
            : _store.Items
                .OrderByDescending(item => item.IsPinned)
                .ThenByDescending(item => item.CopiedAt)
                .ToList();

        if (entries.Count == 0)
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Title = selected.Count >= 2
                ? $"Экспортировать выбранные записи ({selected.Count})"
                : "Экспортировать всю историю",
            Filter = "Текстовый файл (*.txt)|*.txt",
            DefaultExt = "txt",
            AddExtension = true,
            FileName = $"GeniaClipboard_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt",
            OverwritePrompt = true,
            ValidateNames = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var warning = MessageBox.Show(
            this,
            "TXT-файл будет незашифрованным. Продолжить экспорт?",
            "GeniaClipboard",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (warning != DialogResult.Yes)
        {
            return;
        }

        try
        {
            using var writer = new StreamWriter(
                dialog.FileName,
                false,
                new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            for (var index = 0; index < entries.Count; index++)
            {
                writer.Write(entries[index].Text);
                if (index < entries.Count - 1)
                {
                    writer.WriteLine();
                }
            }

            _statusLabel.Text = $"Экспортировано: {entries.Count}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            MessageBox.Show(
                $"Не удалось сохранить TXT-файл.\n\n{ex.Message}",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void SearchBoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && _historyList.Items.Count > 0)
        {
            _historyList.Focus();
            _historyList.Items[0].Selected = true;
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Enter && _historyList.Items.Count > 0)
        {
            _historyList.Items[0].Selected = true;
            _ = PasteSelectedAsync();
            e.SuppressKeyPress = true;
        }
    }

    private void HistoryListOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            foreach (ListViewItem item in _historyList.Items)
            {
                item.Selected = true;
            }

            e.SuppressKeyPress = true;
        }
        else if (e.Control && e.KeyCode == Keys.C)
        {
            _ = CopySelectedAsync();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.F2)
        {
            EditSelectedEntry();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            _ = PasteSelectedAsync();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Delete)
        {
            DeleteSelected();
            e.SuppressKeyPress = true;
        }
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            HideToTray();
            e.SuppressKeyPress = true;
        }
        else if (e.Control && e.KeyCode == Keys.F)
        {
            _searchBox.Focus();
            _searchBox.SelectAll();
            e.SuppressKeyPress = true;
        }
    }

    private void ResizeHistoryColumns()
    {
        if (_historyList.Columns.Count < 4)
        {
            return;
        }

        _historyList.Columns[0].Width = 44;
        _historyList.Columns[2].Width = 140;
        _historyList.Columns[3].Width = 128;
        _historyList.Columns[1].Width = Math.Max(
            220,
            _historyList.ClientSize.Width - 318);
    }

    private void UpdateStatus(int? visibleCount = null)
    {
        if (_store.LastError is not null || _journal.LastError is not null)
        {
            _statusLabel.Text = _store.LastError ?? _journal.LastError;
            _statusLabel.ForeColor = Color.Firebrick;
            return;
        }

        _statusLabel.ForeColor = Color.FromArgb(75, 85, 99);

        if (!_clipboardListenerRegistered)
        {
            _statusLabel.Text = "Резервное слежение за буфером";
            return;
        }

        if (!_hotKeyRegistered)
        {
            _statusLabel.Text = "Глобальный хоткей уже занят";
            return;
        }

        var count = visibleCount ?? _store.Items.Count;
        var selectedCount = _historyList.SelectedItems.Count;
        var selectionSuffix = selectedCount > 1 ? $" · Выбрано: {selectedCount}" : string.Empty;
        var journalSuffix = _journal.Enabled && !_store.IsPrivateSession ? " · TXT" : string.Empty;
        var vault = _store.IsPrivateSession
            ? "Private Session"
            : _store.VaultMode == VaultMode.Portable
                ? "Portable Vault"
                : "Windows Vault";

        _statusLabel.Text = CaptureEnabled
            ? $"{vault} · Записей: {count}{selectionSuffix}{journalSuffix}"
            : $"{vault} · Сбор приостановлен · {count}{selectionSuffix}";
    }

    private static void DrawSearchBorder(object? sender, PaintEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        using var pen = new Pen(Color.FromArgb(148, 163, 184));
        var width = Math.Max(0, control.ClientSize.Width - 1);
        var height = Math.Max(0, control.ClientSize.Height - 1);
        e.Graphics.DrawRectangle(pen, 0, 0, width, height);
    }

    private static string CreatePreview(string text)
    {
        var preview = text
            .Replace("\r\n", " ↵ ")
            .Replace('\r', ' ')
            .Replace('\n', ' ')
            .Replace('\t', ' ')
            .Trim();

        return preview.Length <= 180 ? preview : preview[..180] + "…";
    }

    private static string FormatTimestamp(DateTimeOffset value)
    {
        var local = value.LocalDateTime;
        return local.Date == DateTime.Today
            ? $"Сегодня, {local:HH:mm}"
            : local.ToString("dd.MM.yyyy HH:mm");
    }

    private static void DrawBottomDivider(object? sender, PaintEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        using var pen = new Pen(Color.FromArgb(226, 232, 240));
        e.Graphics.DrawLine(
            pen,
            0,
            control.ClientSize.Height - 1,
            control.ClientSize.Width,
            control.ClientSize.Height - 1);
    }

    private static void DrawTopDivider(object? sender, PaintEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        using var pen = new Pen(Color.FromArgb(226, 232, 240));
        e.Graphics.DrawLine(pen, 0, 0, control.ClientSize.Width, 0);
    }
}
