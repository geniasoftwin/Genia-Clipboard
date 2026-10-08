# GeniaClipboard 0.5.3 — Safe Paste & UI Fixes

[Русский ниже](#русский)

## English

This patch addresses three user-reported issues in 0.5.2:

- **Search alignment:** the native search text box is vertically centered in its row. It retains the native Windows border to avoid previous repaint artifacts.
- **Paste target safety:** auto-paste only targets the application that was in the foreground when GeniaClipboard was opened with the global hotkey. Manually reactivating GeniaClipboard invalidates any previous target, preventing unexpected pastes into an old Notepad or browser window.
- **Clear feedback:** when there is no authorized paste target, or Windows blocks foreground switching, the app explains what happened and how to use Copy + Ctrl+V.
- **Compact status column:** the pinned/sensitive status column is reduced from 44 to 22 pixels, with a single colored marker for combined states. Clipboard content starts 22 pixels closer to the left edge.
- **Branded icons:** the Edit Entry and Portable Vault password dialogs show the same GeniaClipboard icon as the main window.

**Auto-paste:** position the caret in another application, press your GeniaClipboard global hotkey (default Ctrl+Shift+V), choose a clip and click Paste. If you opened GeniaClipboard manually, use Copy and Ctrl+V instead.

The encrypted vault format, Privacy Core capture rules, and settings schema remain unchanged.

## Русский

Исправлены три замечания по GeniaClipboard 0.5.2:

- **Поиск:** однострочное поле поиска теперь выровнено по вертикали; стандартная рамка Windows сохранена, чтобы не возвращались артефакты перерисовки.
- **Безопасная вставка:** кнопка «Вставить» работает только с окном, из которого история была явно открыта глобальной горячей клавишей. При ручном возвращении в GeniaClipboard старая цель сбрасывается, чтобы текст неожиданно не вставился в ранее открытый Блокнот.
- **Понятные сообщения:** если окно для автоматической вставки не определено или Windows запрещает переключение, программа объясняет причину и предлагает «Копировать» + Ctrl+V.
- **Компактная колонка маркеров:** область слева под закрепление и sensitive-маркеры уменьшена с 44 до 22 пикселей; двойное состояние обозначается одной цветной точкой. Текст «Содержимого» начинается на 22 пикселя левее.
- **Иконка:** редактор записи и окно разблокировки Portable Vault теперь используют фирменный значок GeniaClipboard.

**Как пользоваться вставкой:** поставьте курсор в нужном приложении, откройте GeniaClipboard горячей клавишей (по умолчанию Ctrl+Shift+V), выберите запись и нажмите «Вставить». Если открыли окно вручную, используйте «Копировать» и обычное Ctrl+V.

Формат зашифрованной истории, правила захвата и схема настроек не менялись.
