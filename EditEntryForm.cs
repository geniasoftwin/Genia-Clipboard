namespace GeniaClipboard;

internal sealed class EditEntryForm : Form
{
    private readonly TextBox _editor;

    public EditEntryForm(string text)
    {
        Text = "Изменить запись — GeniaClipboard";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(680, 420);
        MinimumSize = new Size(520, 320);
        Font = new Font("Segoe UI", 9F);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
        ShowIcon = true;

        _editor = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            AcceptsReturn = true,
            AcceptsTab = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = true,
            Text = text,
            MaxLength = HistoryStore.MaxTextLength
        };

        var body = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        body.Controls.Add(_editor);

        var saveButton = new Button
        {
            Text = "Сохранить",
            DialogResult = DialogResult.OK,
            Width = 100,
            Height = 30
        };

        var cancelButton = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Width = 90,
            Height = 30
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            Padding = new Padding(0, 8, 10, 8),
            FlowDirection = FlowDirection.RightToLeft
        };
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(cancelButton);

        Controls.Add(body);
        Controls.Add(buttons);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
        UiText.Localize(this);
        Shown += (_, _) =>
        {
            _editor.Focus();
            _editor.SelectionStart = _editor.TextLength;
        };
    }

    public string EditedText => _editor.Text;

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.OK && string.IsNullOrWhiteSpace(_editor.Text))
        {
            MessageBox.Show(
                this,
                UiText.T("Запись не может быть пустой."),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            e.Cancel = true;
        }

        base.OnFormClosing(e);
    }
}
