using System.Runtime.InteropServices;

namespace GeniaClipboard;

internal sealed partial class MainForm : Form
{
    private readonly HistoryStore _store;
    private readonly TextJournalService _journal;
    private readonly AppSettings _settings;
    private const string InternalClipboardFormat = "GeniaClipboard.Internal.v1";
    private const int MaxBatchClipboardLength = 16_000_000;
    private const int MaxMultiPreviewLength = 200_000;
    private readonly TextBox _searchBox;
    private readonly ListView _historyList;
    private readonly TextBox _previewBox;
    private readonly Button _pasteButton;
    private readonly Button _copyButton;
    private readonly Button _editButton;
    private readonly Button _pinButton;
    private readonly Button _deleteButton;
    private readonly Label _statusLabel;
    private readonly NoCopyLabel _hotKeyLabel;
    private readonly ToolTip _toolTip;
    private readonly System.Windows.Forms.Timer _clipboardClearTimer;

    private bool _allowClose;
    private bool _hotKeyRegistered;
    private bool _clipboardListenerRegistered;
    private bool _captureInProgress;
    private uint _lastClipboardSequenceNumber;
    private uint _clipboardSequenceToClear;
    private IntPtr _previousForegroundWindow;
    private uint _pasteTargetProcessId;
    private bool _openingFromShortcut;

    public MainForm(HistoryStore store, TextJournalService journal, AppSettings settings)
    {
        _store = store;
        _journal = journal;
        _settings = settings;

        Text = "GeniaClipboard";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(980, 560);
        MinimumSize = new Size(820, 440);
        KeyPreview = true;
        ShowInTaskbar = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.FromArgb(244, 246, 248);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;

        var appIcon = new PictureBox
        {
            Location = new Point(0, 1),
            Size = new Size(32, 32),
            Image = Icon.ToBitmap(),
            SizeMode = PictureBoxSizeMode.Zoom,
            TabStop = false
        };

        var titleLabel = new NoCopyLabel
        {
            AutoSize = true,
            Location = new Point(40, 0),
            Text = "GeniaClipboard",
            Font = new Font("Segoe UI Semibold", 13F),
            ForeColor = Color.FromArgb(31, 41, 55),
            Margin = new Padding(0)
        };

        var subtitleLabel = new NoCopyLabel
        {
            AutoSize = true,
            Location = new Point(41, 22),
            Text = "Privacy-first история буфера обмена",
            Font = new Font("Segoe UI", 8.25F),
            ForeColor = Color.FromArgb(107, 114, 128)
        };

        _hotKeyLabel = new NoCopyLabel
        {
            Dock = DockStyle.Right,
            Width = 280,
            Text = $"Firewall ON · {HotKeyDefinition.FromSettings(settings).ToDisplayString()}",
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 8.25F),
            ForeColor = Color.FromArgb(16, 120, 80)
        };

        var identityPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 39,
            BackColor = Color.White
        };
        identityPanel.Controls.Add(appIcon);
        identityPanel.Controls.Add(titleLabel);
        identityPanel.Controls.Add(subtitleLabel);
        identityPanel.Controls.Add(_hotKeyLabel);

        // A native single-line TextBox wants its natural height. Center that
        // control vertically inside the search row instead of stretching it to
        // 32 px (which makes both the placeholder and typed text appear too high).
        var searchHost = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 34,
            BackColor = Color.White
        };
        _searchBox = new TextBox
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            PlaceholderText = "Поиск по истории…",
            Font = new Font("Segoe UI", 9.75F),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        _searchBox.TextChanged += (_, _) => RefreshHistoryList();
        _searchBox.KeyDown += SearchBoxOnKeyDown;
        searchHost.Controls.Add(_searchBox);
        void CenterSearchBox()
        {
            _searchBox.SetBounds(
                0,
                Math.Max(0, (searchHost.ClientSize.Height - _searchBox.PreferredSize.Height) / 2),
                searchHost.ClientSize.Width,
                _searchBox.PreferredSize.Height);
        }
        searchHost.Resize += (_, _) => CenterSearchBox();
        CenterSearchBox();

        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 88,
            Padding = new Padding(12, 8, 12, 8),
            BackColor = Color.White
        };
        headerPanel.Controls.Add(searchHost);
        headerPanel.Controls.Add(identityPanel);
        headerPanel.Paint += DrawBottomDivider;

        _historyList = new SubtleRowListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            HideSelection = false,
            MultiSelect = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9.25F),
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            UseCompatibleStateImageBehavior = false
        };
        _historyList.Columns.Add("", 44);
        _historyList.Columns.Add("Содержимое", 500);
        _historyList.Columns.Add("Источник", 150);
        _historyList.Columns.Add("Время", 128);
        _historyList.SelectedIndexChanged += (_, _) => UpdateSelection();
        _historyList.DoubleClick += async (_, _) => await PasteSelectedAsync();
        _historyList.KeyDown += HistoryListOnKeyDown;
        _historyList.Resize += (_, _) => ResizeHistoryColumns();

        _previewBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(31, 41, 55),
            Font = new Font("Segoe UI", 9.25F),
            Margin = new Padding(8)
        };

        var previewBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(9),
            BackColor = Color.White
        };
        previewBody.Controls.Add(_previewBox);

        var previewTitle = new NoCopyLabel
        {
            Dock = DockStyle.Top,
            Height = 27,
            Padding = new Padding(9, 0, 0, 0),
            Text = "ПРОСМОТР",
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI Semibold", 8F),
            ForeColor = Color.FromArgb(100, 116, 139),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var previewPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        previewPanel.Controls.Add(previewBody);
        previewPanel.Controls.Add(previewTitle);

        var splitter = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 270,
            SplitterWidth = 4,
            Panel1MinSize = 130,
            Panel2MinSize = 82,
            BackColor = Color.FromArgb(218, 223, 229)
        };
        splitter.Panel1.Controls.Add(_historyList);
        splitter.Panel2.Controls.Add(previewPanel);

        var bodyCard = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(1),
            BackColor = Color.FromArgb(209, 213, 219)
        };
        bodyCard.Controls.Add(splitter);

        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 8, 10, 8),
            BackColor = Color.FromArgb(244, 246, 248)
        };
        contentPanel.Controls.Add(bodyCard);

        _pasteButton = CreateButton("Вставить", ButtonTone.Primary);
        _pasteButton.Click += async (_, _) => await PasteSelectedAsync();

        _copyButton = CreateButton("Копировать");
        _copyButton.Click += async (_, _) => await CopySelectedAsync();

        _editButton = CreateButton("Изменить");
        _editButton.Click += (_, _) => EditSelectedEntry();

        _pinButton = CreateButton("Закрепить");
        _pinButton.Click += (_, _) => TogglePinned();

        _deleteButton = CreateButton("Удалить");
        _deleteButton.Click += (_, _) => DeleteSelected();

        var exportButton = CreateButton("Экспорт TXT");
        exportButton.Click += (_, _) => ExportToTxt();

        var settingsButton = CreateButton("Настройки");
        settingsButton.Click += (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty);

        var clearButton = CreateButton("Очистить", ButtonTone.Danger);
        clearButton.Click += (_, _) => ClearHistoryWithConfirmation();

        // A fixed two-row footer keeps the vault/status text visible even when
        // eight action buttons exceed the available width at high display DPI.
        // The action row scrolls horizontally rather than overlapping the status.
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoScroll = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(10, 7, 10, 1),
            BackColor = Color.White
        };
        buttonPanel.Controls.AddRange([
            _pasteButton,
            _copyButton,
            _editButton,
            _pinButton,
            _deleteButton,
            exportButton,
            settingsButton,
            clearButton
        ]);

        _statusLabel = new NoCopyLabel
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleRight,
            Padding = new Padding(8, 0, 12, 0),
            ForeColor = Color.FromArgb(75, 85, 99),
            BackColor = Color.White
        };
        var statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 24,
            BackColor = Color.White
        };
        statusPanel.Controls.Add(_statusLabel);

        var footerPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 84,
            BackColor = Color.White
        };
        footerPanel.Controls.Add(buttonPanel);
        footerPanel.Controls.Add(statusPanel);
        footerPanel.Paint += DrawTopDivider;

        Controls.Add(contentPanel);
        Controls.Add(footerPanel);
        Controls.Add(headerPanel);

        _clipboardClearTimer = new System.Windows.Forms.Timer();
        _clipboardClearTimer.Tick += ClipboardClearTimerOnTick;

        _toolTip = new ToolTip
        {
            InitialDelay = 350,
            ReshowDelay = 100,
            AutoPopDelay = 5000
        };
        _toolTip.SetToolTip(_pasteButton, "Вставить выбранный фрагмент — Enter");
        _toolTip.SetToolTip(_copyButton, "Скопировать выбранные записи, по одной на строку");
        _toolTip.SetToolTip(_editButton, "Изменить одну выбранную запись — F2");
        _toolTip.SetToolTip(_pinButton, "Закрепить одну выбранную запись вверху списка");
        _toolTip.SetToolTip(_deleteButton, "Удалить выбранные записи — Delete");
        _toolTip.SetToolTip(exportButton, "Экспорт: несколько выделенных записей или вся история");
        _toolTip.SetToolTip(settingsButton, "Privacy, vault, hotkey и автозапуск");
        _toolTip.SetToolTip(clearButton, "Удалить всю текущую историю");

        KeyDown += OnWindowKeyDown;
        RefreshHistoryList();
    }

    public event EventHandler? SettingsRequested;
}
