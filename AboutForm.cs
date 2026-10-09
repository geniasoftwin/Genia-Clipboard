using System.Diagnostics;

namespace GeniaClipboard;

/// <summary>Non-sensitive product information and official community links.</summary>
internal sealed class AboutForm : Form
{
    public AboutForm()
    {
        Text = UiText.T("О программе — GeniaClipboard");
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(580, 350);
        MinimumSize = new Size(580, 390);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.White;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;

        var version = typeof(AboutForm).Assembly.GetName().Version?.ToString(3) ?? "0.5.7";

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(22, 18, 22, 12),
            BackColor = Color.White,
            ColumnCount = 1,
            AutoScroll = true,
            RowCount = 0
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddLine(content, $"GeniaClipboard v{version}", 18, true, Color.FromArgb(30, 58, 138), 42);
        AddLine(content,
            UiText.IsEnglish
                ? "Privacy-first portable clipboard history for Windows"
                : "Портативная история буфера обмена с заботой о конфиденциальности",
            9.5F, false, Color.FromArgb(75, 85, 99), 38);
        AddLine(content, "Windows 10/11 x64 · MIT License · © 2026 GeniaSoftWin", 9, false,
            Color.FromArgb(65, 75, 90), 30);
        AddLine(content, UiText.T("Ссылки и обратная связь"), 10, true,
            Color.FromArgb(30, 41, 59), 32);

        AddLink(content, UiText.T("Исходный код на GitHub"),
            "https://github.com/geniasoftwin/Genia-Clipboard");
        AddLink(content, UiText.T("Сообщить об ошибке"),
            "https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose");
        AddLink(content, "MIT License",
            "https://github.com/geniasoftwin/Genia-Clipboard/blob/main/LICENSE");

        AddLine(content,
            UiText.T("История текста шифруется локально. TXT-экспорт и журналы не шифруются. Программа не защищает от вредоносного ПО, работающего в вашей сессии Windows."),
            8.5F, false, Color.FromArgb(100, 116, 139), 65);

        var close = new Button
        {
            Text = UiText.T("Закрыть"),
            DialogResult = DialogResult.OK,
            Width = 110,
            Height = 32
        };

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(0, 8, 14, 4),
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.FromArgb(248, 250, 252)
        };
        footer.Controls.Add(close);

        Controls.Add(content);
        Controls.Add(footer);
        AcceptButton = close;
        CancelButton = close;
    }

    private static void AddLine(
        TableLayoutPanel layout, string text, float fontSize, bool strong, Color color, int height)
    {
        var label = new NoCopyLabel
        {
            Text = text,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = height,
            Font = new Font(strong ? "Segoe UI Semibold" : "Segoe UI", fontSize),
            ForeColor = color,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty
        };
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        layout.Controls.Add(label, 0, row);
    }

    private static void AddLink(TableLayoutPanel layout, string label, string uri)
    {
        var link = new LinkLabel
        {
            Text = label,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Height = 28,
            Font = new Font("Segoe UI", 9F),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty,
            LinkColor = Color.FromArgb(37, 99, 235)
        };
        link.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                MessageBox.Show(
                    layout.FindForm(),
                    UiText.IsEnglish ? "Could not open the link." : "Не удалось открыть ссылку.",
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        };
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.Controls.Add(link, 0, row);
    }
}
