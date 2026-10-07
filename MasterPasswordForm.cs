namespace GeniaClipboard;

internal sealed class MasterPasswordForm : Form
{
    private readonly TextBox _passwordBox;

    public MasterPasswordForm()
    {
        Text = "GeniaClipboard — Portable Vault";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        ClientSize = new Size(430, 170);
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            AutoSize = true,
            Location = new Point(20, 18),
            Text = "Введите мастер-пароль Portable Vault",
            Font = new Font("Segoe UI Semibold", 11F)
        };

        var hint = new Label
        {
            AutoSize = false,
            Location = new Point(20, 48),
            Size = new Size(390, 34),
            Text = "Пароль не сохраняется. Он используется только для получения ключа шифрования в памяти.",
            ForeColor = Color.DimGray
        };

        _passwordBox = new TextBox
        {
            Location = new Point(20, 88),
            Width = 390,
            UseSystemPasswordChar = true
        };

        var unlockButton = new Button
        {
            Text = "Разблокировать",
            DialogResult = DialogResult.OK,
            Location = new Point(276, 126),
            Width = 134
        };

        var privateButton = new Button
        {
            Text = "Private Session",
            DialogResult = DialogResult.Ignore,
            Location = new Point(140, 126),
            Width = 128
        };

        var cancelButton = new Button
        {
            Text = "Выход",
            DialogResult = DialogResult.Cancel,
            Location = new Point(40, 126),
            Width = 92
        };

        Controls.AddRange([title, hint, _passwordBox, cancelButton, privateButton, unlockButton]);
        AcceptButton = unlockButton;
        CancelButton = cancelButton;
        Shown += (_, _) => _passwordBox.Focus();
    }

    public string Password => _passwordBox.Text;

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.OK && string.IsNullOrEmpty(_passwordBox.Text))
        {
            MessageBox.Show(
                this,
                "Введите мастер-пароль.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            e.Cancel = true;
        }

        base.OnFormClosing(e);
    }
}
