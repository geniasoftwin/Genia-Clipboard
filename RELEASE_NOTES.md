# GeniaClipboard 0.5.2 — UI Polish & Responsive Layout

[Русский ниже](#русский)

## English

This release polishes the Windows interface without modifying Privacy Core encryption or data storage.

- **Paste** is now a blue primary action, **Clear** is a red destructive action, and the remaining buttons stay neutral.
- Subtle light-gray horizontal separators between clipboard history items, without vertical gridlines.
- Native Windows search border instead of nested custom border painting, addressing header redraw artifacts.
- Two-row footer: action buttons on the upper row, full Vault/status text on a separate lower row.
- Security and History settings use compact spacing and grow the dialog vertically within the available Windows work area to avoid scrollbars where practical.
- Process exclusion examples are clearly labeled as examples, not enabled rules.

Small screens and higher display scaling may still require scrollbars. The encrypted history format, privacy rules and settings schema are unchanged.

## Русский

Обновление **GeniaClipboard 0.5.2** улучшает интерфейс без изменений логики Privacy Core.

- **«Вставить»** — синяя основная кнопка, **«Очистить»** — красная кнопка опасного действия. Остальные кнопки нейтральные.
- Между записями истории появились тонкие, малозаметные светло-серые горизонтальные линии, без вертикальной сетки.
- Поле поиска использует стандартную рамку Windows — больше нет вложенной нарисованной вручную рамки, вызывавшей артефакты.
- Нижняя панель разделена на две строки: сверху кнопки, снизу статус Vault и число записей.
- Вкладки настроек «Безопасность» и «История» компактнее; окно автоматически увеличивается по высоте в пределах доступного рабочего пространства экрана, чтобы по возможности открываться без полосы прокрутки.
- Bitwarden и KeePassXC обозначены как **примеры** исключений, а не уже активные настройки.

На небольших экранах и при высоком масштабе Windows вертикальная прокрутка остаётся резервным вариантом. Формат зашифрованного хранилища, правила защиты и структура настроек не менялись.
