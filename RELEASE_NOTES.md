# GeniaClipboard v0.5.7 — Russian & English UI

[Русская версия ниже](#русский)

## English

GeniaClipboard is a free, MIT-licensed, portable **text-only** clipboard history manager for Windows 10/11 x64. Version 0.5.7 adds bilingual UI while preserving the encrypted history and guarded Paste workflow of v0.5.6.

### What's new

- **Automatic UI language:** Windows interface language **Russian (ru) → Russian**; any other Windows UI language → **English**.
- **Manual language override:** Settings → System → Language: Automatic (Windows), Русский, English. Your choice is saved locally and takes effect **after a restart**.
- **About** dialog with the running version, MIT license, GitHub source and feedback links, and a concise security disclaimer.
- English translation of main history, search, preview, Settings tabs, tray menu, Portable Vault unlock, editor, and key messages.

### Compatibility and privacy

- No changes to the encrypted Vault format, master password, or saved clips. UI localization **never translates or modifies clipboard history content**.
- Local encrypted history uses Windows DPAPI or Portable Vault with a master password. Private Session keeps new entries only in memory.
- Clipboard secret detection is heuristic, not infallible; TXT exports and optional TXT journals are **not encrypted**. Encryption at rest does not protect against software already reading clipboard contents or process memory in your Windows session.
- The EXE is **not Authenticode-signed**. Release assets include a self-contained EXE, portable ZIP and `SHA256SUMS.txt` for verifying both downloads.

### Testing and feedback

Windows CI restored and built the application. The project owner manually verified the five beta checks: Russian UI in Auto mode, persistent manual English selection after restart, About links/version, Portable Vault history/unlock, and paste into Notepad. This does not establish universal compatibility with every application or scaling configuration.

View [authentic English beta screenshots](https://github.com/geniasoftwin/Genia-Clipboard/blob/main/docs/SCREENSHOTS.md) and report reproducible issues via [GitHub Issues](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose). Never post real passwords, private clipboard history or vault files.

**Updating:** Exit GeniaClipboard from the tray, back up the complete `Data` directory, then replace the EXE. Keep your Portable Vault master password.

---

## Русский

GeniaClipboard — бесплатный портативный менеджер **текстовой** истории буфера обмена для Windows 10/11 x64 под лицензией MIT. В версии 0.5.7 добавлен двуязычный интерфейс; защищённая вставка из 0.5.6 и существующий Vault сохранены.

### Что нового

- **Автовыбор языка:** русский язык интерфейса Windows (`ru`) → **русский**; любой другой язык Windows → **английский**.
- **Ручной выбор:** Настройки → Система → Язык: Автоматически (Windows), Русский, English. Выбор сохраняется и применяется **после перезапуска**.
- Окно **«О программе / About»** с версией, MIT License, ссылками на исходники и Issues, пояснением границ защиты.
- Английский перевод истории, поиска, просмотра, вкладок настроек, трея, разблокировки Portable Vault, редактора и основных сообщений.

### Совместимость и безопасность

- Формат Vault, мастер-пароль и сохранённые записи **не изменяются**. Текст буфера обмена не переводится автоматически.
- История шифруется локально через Windows DPAPI или Portable Vault. Private Session держит временные записи только в памяти.
- Sensitive detector может не распознать секрет; TXT-экспорт и опциональный TXT-журнал **не шифруются**. Шифрование на диске не защищает от вредоносного ПО, имеющего доступ к буферу обмена или памяти в текущей сессии Windows.
- EXE пока **без цифровой подписи Authenticode**. В релизе доступны отдельный EXE, portable ZIP и `SHA256SUMS.txt` для обоих файлов.

### Проверка и обратная связь

Windows CI прошёл успешно. Автор вручную проверил пять сценариев: русский Auto, сохранение выбора English после перезапуска, версию и ссылки About, Portable Vault и историю, вставку в Блокнот. Это не гарантия идентичной работы во всех приложениях и масштабах экрана.

[Настоящие скриншоты](https://github.com/geniasoftwin/Genia-Clipboard/blob/main/docs/SCREENSHOTS.md) · [Сообщить об ошибке](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose). Не публикуйте реальные пароли, записи истории или файлы Vault.

**Обновление:** закройте приложение через трей, сохраните резервную копию всей папки `Data`, затем замените EXE. Не теряйте мастер-пароль Portable Vault.
