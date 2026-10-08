# GeniaClipboard 0.5.5 — Selection Safety

[Русский ниже](#русский)

## English

This patch responds to a real Windows test of GeniaClipboard v0.5.4: after pasting multiple clips, the window returned to the tray but reselected the same clips when opened again. Pressing Enter without choosing anything repeated the last paste.

### Fixed

- Selection (including Ctrl/Shift multi-selection) is cleared when hiding the window and upon every new opening.
- List refresh no longer selects the first row automatically.
- Pressing Enter in an empty-selection search field does not paste or implicitly choose the first clip.
- To paste: open the app with the global hotkey, press **Down** to select a clip (or click a row), then **Enter** or **Paste**.
- After a successful paste, the app still hides to the tray.

### Unchanged

The authenticated encrypted vault, history persistence, settings schema, keyboard shortcut, and safe destination-window validation are unchanged.

## Русский

Патч по результатам реального тестирования GeniaClipboard 0.5.4: после множественной вставки приложение скрывалось в трей, но при следующем вызове выделяло прежние записи. Нажатие Enter без нового выбора повторяло прошлую вставку.

### Исправлено

- При скрытии в трей и каждом новом открытии очищается выделение, включая множественное Ctrl/Shift.
- После обновления списка первая запись больше не выделяется автоматически.
- Enter без выбранной записи не вставляет текст и не выбирает первую строку.
- Для вставки откройте историю горячей клавишей, нажмите **↓** для выбора записи (или щёлкните её), затем **Enter** или «Вставить».
- После успешной вставки окно по-прежнему уходит в трей.

### Без изменений

Зашифрованное хранилище и его формат, сохранение истории, схема настроек, глобальная комбинация клавиш и защита от вставки в чужое окно не менялись.
