# Public beta guide / Руководство по бета-тестированию

## English

**GeniaClipboard** is an MIT-licensed, local-first text clipboard history manager for Windows 10/11 x64.

1. Download the current [release ZIP](https://github.com/geniasoftwin/Genia-Clipboard/releases) and verify the SHA-256 checksum if desired.
2. Extract it to a folder you control and run `GeniaClipboard.exe`. The binary is self-contained.
3. Use the configured shortcut (default `Ctrl+Shift+V`) or the tray icon to open history.
4. Select a text entry explicitly, then use **Copy** (clipboard only) or **Paste** (tries to insert in the most recently active external application).
5. If automatic Paste cannot confirm a reliable target, use Copy and manually press `Ctrl+V`.

**Upgrading:** Exit the application from the tray. Back up the complete `Data` folder, including vault files, before replacing the EXE. Windows Vault is tied to the Windows user profile; Portable Vault needs its master password.

**Screenshots and feedback:** Open a [GitHub Issue](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose). Include a version, Windows edition, scaling, reproduction steps and expected/actual results. Use dummy text. Do not share private clipboard captures, passwords, tokens, `history.gch` or `history.key`.

**Known limitations:** Text only; heuristic sensitive-data detection; plaintext TXT exports/journals; no Authenticode code signing. Encrypted storage protects at rest, not against malware running as you.

## Русский

**GeniaClipboard** — менеджер истории текста для Windows 10/11 x64, локальный и открытый под MIT License.

1. Скачайте [ZIP последнего релиза](https://github.com/geniasoftwin/Genia-Clipboard/releases), при необходимости проверьте SHA-256.
2. Распакуйте в удобную папку и запустите `GeniaClipboard.exe` — установленный .NET runtime на ПК не нужен.
3. Откройте историю хоткеем (по умолчанию `Ctrl+Shift+V`) или через трей.
4. Явно выделите запись; **«Копировать»** только помещает её в буфер, а **«Вставить»** пытается вставить в последнее активное внешнее приложение.
5. Если надёжной цели для автоматической вставки нет, используйте «Копировать» и ручное `Ctrl+V`.

**Обновление:** выйдите через трей. Перед заменой EXE сохраните резервную копию всей папки `Data`, включая файлы vault. Windows Vault привязан к пользователю Windows, Portable Vault требует мастер-пароль.

**Скриншоты и обратная связь:** создайте [GitHub Issue](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose), укажите версию, Windows, масштаб экрана, шаги и ожидаемый/фактический результат. Используйте только тестовые тексты. Не публикуйте настоящие пароли, токены, личную историю и файлы `history.gch` / `history.key`.

**Ограничения:** только текст; распознавание секретов эвристическое; TXT-журнал/экспорт не шифруются; Authenticode-подписи нет. Шифрование защищает историю на диске, но не от malware, работающего под вашим пользователем.
