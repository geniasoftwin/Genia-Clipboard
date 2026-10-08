# GeniaClipboard 0.5.6 — Unified Paste (Public Beta)

[Русский ниже](#русский)

## English

This update focuses on one important real-world behavior: **Paste must work consistently whether clipboard history is opened with the global shortcut, from the tray, or manually**.

- Tracks the most recently focused external application using Windows foreground notifications.
- Excludes GeniaClipboard windows and the Windows taskbar/shell as destinations.
- Validates the destination window, owning process and current foreground focus immediately before Ctrl+V.
- If a trustworthy destination is unavailable, does not send keystrokes into an unrelated program; offers Copy + manual Ctrl+V.
- Keeps the 0.5.5 fix: opening the history does not restore a previous single/multiple selection. Select a clip for each new paste.

### Tested on Windows

The project owner confirmed that a real Windows beta build can **Copy** a selected clip, switch focus to Notepad, return to GeniaClipboard and use **Paste** to insert the text into Notepad. This is a manual workflow check, not an automated guarantee for all applications and focus combinations.

### Community preview

GeniaClipboard is a free, open-source Windows clipboard manager, MIT-licensed, with an encrypted local history (Windows DPAPI or a portable master-password vault), a memory-only Private Session, optional per-process exclusions, search and entry editing.

This is **text-only** software. Sensitive-text recognition is heuristic, not a security guarantee. Optional TXT journals and exports are **not encrypted**. The app cannot protect against malware with access to your Windows session or clipboard.

**Help test this beta:** Please report reproducible issues via GitHub Issues and include the version, Windows build, steps and expected vs actual behavior. Never upload real secrets, passwords, vault files or private clipboard captures.

## Русский

Версия 0.5.6 исправляет один из главных сценариев: **«Вставить» должно работать одинаково независимо от того, открыта история горячей клавишей, через трей или вручную**.

- Программа отслеживает недавно активное внешнее окно через события Windows.
- Собственные окна GeniaClipboard, панель задач и системная оболочка Windows не считаются окнами назначения.
- Перед Ctrl+V проверяются конкретное окно, его процесс и фактический фокус.
- Если цель не определена надёжно, программа не отправляет клавиши в случайное окно, а предлагает «Копировать» + Ctrl+V.
- Исправление 0.5.5 сохраняется: при новом открытии записи не выделены, пока пользователь не выберет их снова.

### Проверено на Windows

Автор подтвердил на тестовой Windows-сборке сценарий: выделить запись → «Копировать» → переключиться в Блокнот → вернуться в GeniaClipboard → «Вставить». Текст успешно вставляется в Блокнот. Это ручная проверка конкретного сценария, а не обещание одинакового результата во всех программах.

### Открытое бета-тестирование

GeniaClipboard — бесплатный менеджер истории текста для Windows с открытым исходным кодом под MIT License. История шифруется локально (Windows DPAPI или Portable Vault с мастер-паролем), есть Private Session только в RAM, поиск, редактор и исключения приложений.

На текущем этапе поддерживается **только текст**. Распознавание секретов работает эвристически. TXT-журнал и TXT-экспорт не шифруются. Программа не защищает от malware, уже имеющего доступ к Windows-сессии и системному буферу.

**Как помочь:** присылайте воспроизводимые ошибки через GitHub Issues с версией, Windows, шагами и ожидаемым/фактическим поведением. Не загружайте настоящие пароли, токены, файлы Vault или содержимое личного буфера.
