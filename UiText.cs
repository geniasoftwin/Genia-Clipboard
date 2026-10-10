using System.Globalization;

namespace GeniaClipboard;

/// <summary>
/// Display-only localization: never translate user clipboard content or persisted vault data.
/// The application language is chosen on startup; changing the setting requires a restart.
/// </summary>
internal static class UiText
{
    private static bool _english;

    private static readonly Dictionary<string, string> English = new(StringComparer.Ordinal)
    {
        ["Privacy-first история буфера обмена"] = "Privacy-first clipboard history",
        ["Поиск по истории…"] = "Search clipboard history…",
        ["Содержимое"] = "Content",
        ["Источник"] = "Source",
        ["Время"] = "Time",
        ["ПРОСМОТР"] = "PREVIEW",
        ["Вставить"] = "Paste",
        ["Копировать"] = "Copy",
        ["Изменить"] = "Edit",
        ["Закрепить"] = "Pin",
        ["Открепить"] = "Unpin",
        ["Удалить"] = "Delete",
        ["Экспорт TXT"] = "Export TXT",
        ["Настройки"] = "Settings",
        ["Очистить"] = "Clear",
        ["О программе"] = "About",
        ["Вставить выбранный фрагмент — Enter"] = "Paste selected entries — Enter",
        ["Скопировать выбранные записи, по одной на строку"] = "Copy selected entries, separated by newlines",
        ["Изменить одну выбранную запись — F2"] = "Edit a selected entry — F2",
        ["Закрепить одну выбранную запись вверху списка"] = "Pin selected entry to the top",
        ["Удалить выбранные записи — Delete"] = "Delete selected entries — Delete",
        ["Экспорт: несколько выделенных записей или вся история"] = "Export selected entries or the complete history",
        ["Privacy, vault, hotkey и автозапуск"] = "Privacy, vault, shortcut and startup",
        ["Удалить всю текущую историю"] = "Clear the current history",
        ["Настройки — GeniaClipboard"] = "Settings — GeniaClipboard",
        ["Безопасность"] = "Security",
        ["История"] = "History",
        ["Горячие клавиши"] = "Hotkeys",
        ["Система"] = "System",
        ["Зашифрованное хранилище"] = "Encrypted vault",
        ["Portable Vault (мастер-пароль)"] = "Portable Vault (master password)",
        ["Режим Vault"] = "Vault mode",
        ["Windows Vault привязан к текущей учётной записи Windows. Portable Vault можно переносить между компьютерами вместе с зашифрованной историей."] = "Windows Vault is tied to this Windows user account. Portable Vault can be moved between computers together with encrypted history.",
        ["Новый мастер-пароль"] = "New master password",
        ["Повтор пароля"] = "Confirm password",
        ["Чтобы оставить существующий пароль Portable Vault, не заполняйте поля. При переходе в Portable Vault задайте новый пароль минимум из 10 символов."] = "Leave the fields blank to keep your Portable Vault password. When switching to Portable Vault, enter a new password of at least 10 characters.",
        ["Запускать в Private Session"] = "Start in Private Session",
        ["Приватный запуск"] = "Private startup",
        ["Private Session сохраняет временные записи только в памяти и не записывает их в TXT-журнал."] = "Private Session stores temporary entries only in memory and never writes them to the TXT journal.",
        ["Защита конфиденциальных данных"] = "Sensitive data protection",
        ["Уважать privacy-маркеры Windows"] = "Respect Windows privacy markers",
        ["Маркеры Windows"] = "Windows markers",
        ["Распознавать токены и секреты"] = "Detect tokens and secrets",
        ["Sensitive detector"] = "Sensitive detector",
        ["Удаление секретов, сек."] = "Sensitive expiry, sec.",
        ["0 = не удалять автоматически. Распознавание секретов эвристическое и не гарантирует обнаружение каждого пароля или токена."] = "0 = never expire automatically. Secret detection is heuristic and cannot guarantee detection of every password or token.",
        ["Хранение истории"] = "History retention",
        ["Лимит записей"] = "Entry limit",
        ["Удалять через, дней"] = "Expire after, days",
        ["0 = хранить без ограничения по времени. Закреплённые записи не удаляются по общему лимиту и сроку хранения."] = "0 = keep indefinitely. Pinned entries are excluded from the general limit and retention period.",
        ["Системный буфер обмена"] = "System clipboard",
        ["Автоочистка, сек."] = "Auto-clear, sec.",
        ["0 = выключено. Автоочистка удалит содержимое буфера, только если оно не изменилось с момента копирования."] = "0 = disabled. Auto-clear removes clipboard contents only if they have not changed since copying.",
        ["Исключения приложений"] = "Excluded applications",
        ["Не сохранять содержимое буфера из указанных процессов. Введите имена процессов без .exe, по одному на строку; например: Bitwarden или KeePassXC. Примеры не являются активными исключениями."] = "Do not capture clipboard content from the listed processes. Enter process names without .exe, one per line, e.g. Bitwarden or KeePassXC. Examples are not active exclusions.",
        ["Введите имя процесса (например, Bitwarden)"] = "Enter a process name (e.g. Bitwarden)",
        ["Имя процесса помогает фильтровать приложения, но не является надёжным подтверждением происхождения данных."] = "Process names help with filtering but are not a reliable proof of data origin.",
        ["Открытие истории"] = "Open clipboard history",
        ["Для глобального сочетания выберите хотя бы один модификатор. Если комбинация уже занята другим приложением, текущая будет сохранена."] = "Select at least one modifier for the global shortcut. If the combination is in use by another app, the current one is retained.",
        ["Запуск программы"] = "Application startup",
        ["Запускать вместе с Windows"] = "Start with Windows",
        ["Автозапуск"] = "Autostart",
        ["Используется автозапуск только для текущего пользователя Windows. Права администратора не требуются."] = "Autostart applies only to the current Windows user; administrator privileges are not required.",
        ["Экспорт и журнал"] = "Export and journal",
        ["Вести ежедневный TXT-журнал"] = "Keep a daily TXT journal",
        ["TXT-журнал"] = "TXT journal",
        ["Внимание: TXT-журнал не шифруется. Sensitive-записи и записи Private Session в него не добавляются."] = "Warning: TXT journals are not encrypted. Sensitive entries and Private Session records are excluded.",
        ["Сохранить"] = "Save",
        ["Отмена"] = "Cancel",
        ["Portable Vault переносится между компьютерами. Не потеряйте мастер-пароль: без него историю нельзя восстановить."] = "Portable Vault can be moved between computers. Keep your master password: history cannot be recovered without it.",
        ["Windows Vault использует DPAPI текущей учётной записи Windows и не требует мастер-пароля."] = "Windows Vault uses DPAPI for the current Windows user and does not require a master password.",
        ["Изменить запись — GeniaClipboard"] = "Edit entry — GeniaClipboard",
        ["Запись не может быть пустой."] = "An entry cannot be empty.",
        ["Введите мастер-пароль Portable Vault"] = "Enter your Portable Vault master password",
        ["Пароль не сохраняется. Он используется только для получения ключа шифрования в памяти."] = "The password is never stored. It is used only to derive the encryption key in memory.",
        ["Разблокировать"] = "Unlock",
        ["Выход"] = "Exit",
        ["Введите мастер-пароль."] = "Enter your master password.",
        ["Открыть"] = "Open",
        ["Сохранять скопированное"] = "Capture clipboard text",
        ["Private Session — только память"] = "Private Session — memory only",
        ["Автожурнал TXT"] = "TXT auto-journal",
        ["Настройки…"] = "Settings…",
        ["Очистить историю"] = "Clear history",
        ["Clipboard Firewall работает локально. История хранится в зашифрованном vault."] = "Clipboard Firewall runs locally. History is stored in an encrypted vault.",
        ["Для глобального хоткея выберите хотя бы один модификатор."] = "Choose at least one modifier for the global hotkey.",
        ["Для перехода в Portable Vault задайте мастер-пароль."] = "Enter a master password before switching to Portable Vault.",
        ["Мастер-пароль должен содержать минимум 10 символов."] = "The master password must contain at least 10 characters.",
        ["Мастер-пароли не совпадают."] = "The master passwords do not match.",
        ["Не удалось изменить автозапуск."] = "Failed to change autostart.",
        ["Не удалось изменить режим vault."] = "Failed to change vault mode.",
        ["Не удалось открыть постоянную историю."] = "Failed to open persistent history.",
        ["В Private Session постоянная зашифрованная история будет временно скрыта. Новые записи останутся только в памяти и исчезнут при выходе. Продолжить?"] = "Private Session temporarily hides your encrypted history. New entries are kept only in memory and discarded when you exit. Continue?",
        ["Завершить Private Session? Вся временная история этой сессии будет удалена без сохранения."] = "End Private Session? All temporary session history will be discarded without saving.",
        ["Удалить всю временную историю Private Session?"] = "Delete all temporary Private Session history?",
        ["Удалить всю зашифрованную историю, включая закреплённые записи?"] = "Delete all encrypted history, including pinned entries?",
        ["Резервное слежение за буфером"] = "Clipboard polling fallback",
        ["Глобальный хоткей уже занят"] = "Global hotkey is already in use",
        ["Скопировано"] = "Copied",
        ["Буфер обмена сейчас занят другой программой. Попробуйте ещё раз."] = "Another program is using the clipboard. Please try again.",
        ["Не удалось сохранить изменённую запись."] = "Could not save edited entry.",
        ["Экспортировать всю историю"] = "Export all history",
        ["Текстовый файл (*.txt)|*.txt"] = "Text file (*.txt)|*.txt",
        ["TXT-файл будет незашифрованным. Продолжить экспорт?"] = "The TXT file will not be encrypted. Continue exporting?",
        ["Не удалось определить недавнее активное окно для вставки.\n\nПерейдите в нужное приложение, установите курсор и вернитесь в GeniaClipboard любым способом — горячей клавишей или через трей.\n\nДля вставки вручную используйте «Копировать» и Ctrl+V."] = "Could not determine a recent active window for pasting.\n\nOpen the target application, place the caret and return to GeniaClipboard using the hotkey or tray.\n\nTo paste manually, use Copy and Ctrl+V.",
        ["Windows не разрешила переключиться в целевое окно. Текст уже скопирован: перейдите в нужное приложение и нажмите Ctrl+V."] = "Windows could not activate the target window. The text was copied; switch to your application and press Ctrl+V.",
        ["GeniaClipboard — Вставить"] = "GeniaClipboard — Paste",
        ["Выбранные записи слишком велики для одного пакетного копирования. Уменьшите выбор или используйте экспорт в TXT."] = "The selection is too large for a single clipboard operation. Select fewer entries or export them to TXT.",
        ["… предпросмотр сокращён …"] = "… preview truncated …",
        ["GeniaClipboard уже запущен. Найдите его значок в системном трее."] = "GeniaClipboard is already running. Find its icon in the system tray.",
        ["Не удалось разблокировать Portable Vault."] = "Could not unlock Portable Vault.",
        ["Не удалось открыть Windows Vault."] = "Could not open Windows Vault.",
        ["GeniaClipboard — миграция"] = "GeniaClipboard — migration",
        ["Настройки не сохранены: "] = "Could not save settings: ",
        ["Настройки не прочитаны: "] = "Could not load settings: ",
        ["Файл настроек слишком большой."] = "Settings file is too large.",
        ["О приложении"] = "About",
        ["Язык интерфейса"] = "Interface language",
        ["Язык"] = "Language",
        ["Автоматически (Windows)"] = "Automatic (Windows)",
        ["Язык интерфейса изменится после перезапуска GeniaClipboard. Для сохранения зашифрованной истории выйдите через трей и запустите приложение снова."] = "Interface language will change after you restart GeniaClipboard. Exit through the tray and start the application again; encrypted history stays intact.",
        ["Ссылки и обратная связь"] = "Links and feedback",
        ["Исходный код на GitHub"] = "Source code on GitHub",
        ["Сообщить об ошибке"] = "Report an issue",
        ["MIT License"] = "MIT License",
        ["О программе — GeniaClipboard"] = "About GeniaClipboard",
        ["История текста шифруется локально. TXT-экспорт и журналы не шифруются. Программа не защищает от вредоносного ПО, работающего в вашей сессии Windows."] = "Text history is encrypted locally. TXT exports and journals are not encrypted. This app cannot protect against malware running in your Windows session.",
        ["Закрыть"] = "Close",
    };

    public static string Language { get; private set; } = "ru";
    public static bool IsEnglish => _english;

    public static void Configure(string? requested)
    {
        Language = requested switch
        {
            "ru" => "ru",
            "en" => "en",
            _ => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "ru" : "en"
        };
        _english = Language == "en";
    }

    public static string T(string russian) =>
        _english && English.TryGetValue(russian, out var translated) ? translated : russian;

    // Translate storage-layer errors only when they are displayed. Exception
    // messages or vault contents themselves are never changed.
    private static readonly Dictionary<string, string> VaultErrors = new(StringComparer.Ordinal)
    {
        ["Не удалось расшифровать историю. Проверьте мастер-пароль или Windows-профиль."] =
            "Could not decrypt history. Check the master password or Windows user profile.",
        ["Режим хранилища нельзя менять во время Private Session."] =
            "The vault mode cannot be changed during Private Session.",
        ["Неизвестный режим хранилища."] = "Unknown vault mode.",
        ["Для Portable Vault задайте мастер-пароль не короче 10 символов."] =
            "Portable Vault requires a master password of at least 10 characters.",
        ["Постоянный vault не разблокирован. Перезапустите GeniaClipboard и разблокируйте vault мастер-паролем."] =
            "The persistent vault is locked. Restart GeniaClipboard and unlock it with your master password.",
        ["Не удалось создать зашифрованное хранилище."] = "Could not create encrypted vault.",
        ["Файл ключа Windows Vault отсутствует."] = "Windows Vault key file is missing.",
        ["Файл ключа Windows Vault повреждён."] = "Windows Vault key file is damaged.",
        ["Windows Vault вернул ключ неверного размера."] = "Windows Vault returned a key of incorrect size.",
        ["Portable Vault заблокирован."] = "Portable Vault is locked.",
        ["Хранилище не разблокировано."] = "The vault is locked.",
        ["Режим хранилища изменён без корректной миграции."] =
            "The vault mode changed without a valid migration.",
        ["Параметры Portable Vault не совпадают."] = "Portable Vault parameters do not match.",
        ["Старый history.json слишком большой для безопасной миграции."] =
            "The old history.json is too large for safe migration.",
        ["Хранилище не разблокировано; запись отменена."] =
            "The vault is locked; saving was cancelled.",
        ["Повреждён заголовок зашифрованной истории."] =
            "The encrypted history header is corrupted.",
        ["Некорректная соль зашифрованной истории."] =
            "The encrypted history salt is invalid.",
        ["Некорректная соль Portable Vault."] =
            "The Portable Vault salt is invalid.",
        ["Windows Vault содержит неожиданные параметры ключа."] =
            "Windows Vault contains unexpected key parameters.",
        ["Мастер-пароль не задан."] = "The master password was not provided.",
        ["Некорректный размер зашифрованной истории."] =
            "Encrypted history has an invalid length.",
        ["Размер зашифрованной истории не совпадает с заголовком."] =
            "The encrypted history size does not match its header.",
        ["Файл зашифрованной истории не найден."] = "Encrypted history file was not found.",
        ["Неизвестный формат зашифрованной истории."] =
            "Unknown encrypted history format.",
        ["Некорректный ключ хранилища."] = "Invalid vault key.",
        ["Зашифрованная история неожиданно обрывается."] =
            "The encrypted history is unexpectedly truncated."
    };

    private static readonly (string Russian, string English)[] ErrorPrefixes =
    [
        ("Не удалось открыть хранилище GeniaClipboard: ", "Could not open GeniaClipboard vault: "),
        ("Не удалось изменить режим хранилища: ", "Could not change vault mode: "),
        ("Не удалось вернуться к постоянной истории: ", "Could not return to persistent history: "),
        ("История не сохранена: ", "History was not saved: "),
        ("История уже зашифрована, но старый незашифрованный history.json не удалось удалить: ",
         "History was encrypted, but the old plaintext history.json could not be deleted: "),
        ("TXT-журнал не записан: ", "TXT journal could not be written: "),
        ("Не удалось изменить автозапуск: ", "Could not change autostart: "),
        ("Не удалось открыть раздел автозапуска текущего пользователя.",
         "Could not open the current user's autostart registry key.")
    ];

    public static string Error(string? message)
    {
        if (string.IsNullOrEmpty(message)) return string.Empty;
        if (!_english) return message;
        if (VaultErrors.TryGetValue(message, out var translated)) return translated;

        foreach (var (russian, english) in ErrorPrefixes)
        {
            if (message.StartsWith(russian, StringComparison.Ordinal))
                return english + message[russian.Length..];
        }
        return T(message);
    }

    public static void Localize(Control root)
    {
        if (!_english) return;
        // Do not touch TextBox.Text (clipboard data, passwords and user-entered
        // process names) or user-defined content in other editable controls.
        if (root is Form or Label or Button or CheckBox or TabPage or GroupBox)
            root.Text = T(root.Text);
        if (root is TextBox box) box.PlaceholderText = T(box.PlaceholderText);
        if (root is ListView list)
        {
            foreach (ColumnHeader column in list.Columns) column.Text = T(column.Text);
        }
        if (root is ComboBox combo)
        {
            for (var i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is string value) combo.Items[i] = T(value);
            }
        }
        foreach (Control child in root.Controls) Localize(child);
    }
}
