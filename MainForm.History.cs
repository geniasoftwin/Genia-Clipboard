using System.Runtime.InteropServices;

namespace GeniaClipboard;

internal sealed partial class MainForm
{
    private async void CaptureClipboardTextAsync()
    {
        if (!CaptureEnabled || _captureInProgress)
        {
            return;
        }

        var sequenceNumber = NativeMethods.GetClipboardSequenceNumber();
        if (sequenceNumber != 0 && sequenceNumber == _lastClipboardSequenceNumber)
        {
            return;
        }

        var source = ClipboardSourceInfo.Capture();
        if (source.IsOwnProcess)
        {
            _lastClipboardSequenceNumber = sequenceNumber;
            return;
        }

        if (source.IsExcluded(_settings))
        {
            _lastClipboardSequenceNumber = sequenceNumber;
            return;
        }

        _captureInProgress = true;
        try
        {
            for (var attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    var dataObject = Clipboard.GetDataObject();
                    if (dataObject is null)
                    {
                        _lastClipboardSequenceNumber = sequenceNumber;
                        return;
                    }

                    if (dataObject.GetDataPresent(InternalClipboardFormat, autoConvert: false))
                    {
                        _lastClipboardSequenceNumber = sequenceNumber;
                        return;
                    }

                    if (ClipboardPrivacyPolicy.ShouldIgnore(
                            dataObject,
                            _settings.RespectWindowsPrivacyMarkers))
                    {
                        _lastClipboardSequenceNumber = sequenceNumber;
                        return;
                    }

                    if (!dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true) &&
                        !dataObject.GetDataPresent(DataFormats.Text, autoConvert: true))
                    {
                        _lastClipboardSequenceNumber = sequenceNumber;
                        return;
                    }

                    var text = dataObject.GetData(DataFormats.UnicodeText, autoConvert: true) as string
                               ?? dataObject.GetData(DataFormats.Text, autoConvert: true) as string
                               ?? string.Empty;

                    _lastClipboardSequenceNumber = sequenceNumber;

                    var isSensitive =
                        _settings.DetectSensitiveText &&
                        SensitiveDataDetector.LooksSensitive(text);

                    if (_store.AddOrPromote(
                            text,
                            source.ProcessName,
                            source.WindowTitle,
                            isSensitive,
                            _settings.SensitiveExpireSeconds,
                            out var newEntry))
                    {
                        if (newEntry is not null &&
                            !newEntry.IsSensitive &&
                            !_store.IsPrivateSession)
                        {
                            _journal.Append(newEntry);
                        }

                        ScheduleClipboardClear(sequenceNumber);
                        RefreshHistoryList();
                    }

                    return;
                }
                catch (ExternalException)
                {
                    await Task.Delay(40 * (attempt + 1));
                }
            }
        }
        finally
        {
            _captureInProgress = false;
        }
    }

    private void RefreshHistoryList()
    {
        var selectedIds = SelectedEntries.Select(entry => entry.Id).ToHashSet();
        var query = _searchBox.Text.Trim();
        var items = _store.Items
            .Where(item =>
                query.Length == 0 ||
                item.Text.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(item.SourceProcess) &&
                 item.SourceProcess.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
            .OrderByDescending(item => item.IsPinned)
            .ThenByDescending(item => item.CopiedAt)
            .ToList();

        _historyList.BeginUpdate();
        _historyList.Items.Clear();

        foreach (var entry in items)
        {
            var marker = entry switch
            {
                { IsPinned: true, IsSensitive: true } => "●",
                { IsPinned: true } => "●",
                { IsSensitive: true } => "!",
                _ => string.Empty
            };

            var listItem = new ListViewItem(marker)
            {
                Tag = entry,
                UseItemStyleForSubItems = false
            };
            listItem.SubItems.Add(CreatePreview(entry.Text));
            listItem.SubItems.Add(string.IsNullOrWhiteSpace(entry.SourceProcess) ? "—" : entry.SourceProcess);
            listItem.SubItems.Add(FormatTimestamp(entry.CopiedAt));

            var rowColor = entry.IsSensitive
                ? Color.FromArgb(255, 247, 237)
                : entry.IsPinned
                    ? Color.FromArgb(239, 246, 255)
                    : Color.White;

            foreach (ListViewItem.ListViewSubItem subItem in listItem.SubItems)
            {
                subItem.BackColor = rowColor;
            }

            listItem.SubItems[0].ForeColor = entry.IsSensitive
                ? Color.FromArgb(194, 65, 12)
                : Color.FromArgb(37, 99, 235);
            listItem.SubItems[1].ForeColor = Color.FromArgb(31, 41, 55);
            listItem.SubItems[2].ForeColor = Color.FromArgb(71, 85, 105);
            listItem.SubItems[3].ForeColor = Color.FromArgb(100, 116, 139);
            _historyList.Items.Add(listItem);

            if (selectedIds.Contains(entry.Id))
            {
                listItem.Selected = true;
            }
        }

        _historyList.EndUpdate();

        // An empty selection is intentional. Never implicitly select the first
        // item after reopening or a refresh, since Enter must not repeat a
        // previous paste or choose a clip without user action.
        UpdateSelection();
        UpdateStatus(items.Count);
    }

    private IReadOnlyList<ClipboardEntry> SelectedEntries =>
        _historyList.SelectedItems
            .Cast<ListViewItem>()
            .OrderBy(item => item.Index)
            .Select(item => item.Tag as ClipboardEntry)
            .Where(entry => entry is not null)
            .Cast<ClipboardEntry>()
            .ToList();

    private ClipboardEntry? SelectedEntry => SelectedEntries.FirstOrDefault();

    private bool TryBuildSelectedText(out string text)
    {
        var selected = SelectedEntries;
        var totalLength = 0L;
        foreach (var entry in selected)
        {
            totalLength += entry.Text.Length;
            if (selected.Count > 1)
            {
                totalLength += Environment.NewLine.Length;
            }

            if (totalLength > MaxBatchClipboardLength)
            {
                text = string.Empty;
                MessageBox.Show(
                    "Выбранные записи слишком велики для одного пакетного копирования. Уменьшите выбор или используйте экспорт в TXT.",
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }
        }

        text = string.Join(Environment.NewLine, selected.Select(entry => entry.Text));
        return true;
    }

    private static string BuildMultiPreview(IReadOnlyList<ClipboardEntry> selected)
    {
        var builder = new System.Text.StringBuilder(Math.Min(MaxMultiPreviewLength, 4096));
        for (var index = 0; index < selected.Count; index++)
        {
            if (index > 0)
            {
                builder.AppendLine().AppendLine();
            }

            var remaining = MaxMultiPreviewLength - builder.Length;
            if (remaining <= 0)
            {
                break;
            }

            var value = selected[index].Text;
            builder.Append(value.AsSpan(0, Math.Min(value.Length, remaining)));
        }

        if (builder.Length >= MaxMultiPreviewLength)
        {
            builder.AppendLine().Append("… предпросмотр сокращён …");
        }

        return builder.ToString();
    }

    private void UpdateSelection()
    {
        var selected = SelectedEntries;
        var entry = selected.Count == 1 ? selected[0] : null;
        _previewBox.Text = selected.Count switch
        {
            0 => string.Empty,
            1 => selected[0].Text,
            _ => BuildMultiPreview(selected)
        };

        var hasSelection = selected.Count > 0;
        _pasteButton.Enabled = hasSelection;
        _copyButton.Enabled = hasSelection;
        _editButton.Enabled = selected.Count == 1;
        _pinButton.Enabled = selected.Count == 1;
        _deleteButton.Enabled = hasSelection;
        _pinButton.Text = entry?.IsPinned == true ? "Открепить" : "Закрепить";
        UpdateStatus();
    }
}
