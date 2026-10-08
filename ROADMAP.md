# GeniaClipboard Roadmap

[Русский ниже](#русский)

## Positioning / Product direction

**Clipboard Firewall + Encrypted Vault + Text Workbench** for Windows — a local-first, portable alternative for people who primarily work with text. GeniaClipboard is not a feature-for-feature Ditto or CopyQ clone.

This roadmap contains **plans, not shipped features**. Items must be independently tested and checked off before they appear in README feature claims.

## Current 0.5.6 beta candidate

- Unified safe Paste when opening history by shortcut, tray or taskbar.
- Recent eligible external window tracking, window/process validation and no-target fallback.
- Selection reset on reopen from 0.5.5.
- Bilingual open-source documentation and beta testing instructions.
- Authentic sanitized screenshots for the public repository, pending image upload.

## 0.5.7 — About & localization (first priority after beta)

- **About** dialog showing name, version, MIT license, supported Windows version, GitHub and Issues links, and a concise encryption/threat-model disclaimer.
- RU/EN application UI via centralized localization resources (ideally .resx), with auto-detect and manual language selection in Settings → System.
- All form labels, errors, menus, tooltips, accessibility text and system-tray messages localized.
- Fall back to English for missing translations. Never alter saved entry text or encrypted vault contents.
- Validate long translated strings at 100%, 125% and 150% display scaling.

## 0.5.8 — Daily UX

- Right-click context menu (Paste, Copy, Edit, Pin, Delete).
- Simplified action bar: primary Paste/Copy remain visible, settings in header and export in an overflow menu.
- Entry type badges for **text / URL / code / sensitive** using safe heuristics; this is presentation, not secret detection.
- Optional Always on Top.
- Inline preview editor (with explicit Save / Cancel and clear unsaved-change behavior).
- Search match highlighting, if the chosen list control can support it accessibly without high rendering cost.
- Date grouping (Today, Yesterday, Last week, Older) with pinned entries remaining discoverable.

## 0.6.x — Rich Clipboard (high-priority new capability)

- Images with preview, dimensions and size limits.
- Explorer file lists with explicit formats and **no automatic opening/execution of copied files**.
- HTML/RTF copy/paste with strict validation and plain-text fallback.
- Distinct retention quota for binary data, encrypted-at-rest storage, defensive parsing and deletion.
- Cross-format clipboard restoration and migration tests before enabling rich capture by default.

## 0.6.x+ — Trust and portability

- SHA-256 verification instructions and checksums for the ZIP and EXE in releases.
- Portable Vault encrypted import/export with authenticated envelope and no accidental plaintext fallback.
- Optional vault auto-lock after idle (5/15/30/60 min), including secure key disposal and safe unlock UI.
- Optional clear system clipboard on exit (explicit opt-in, never silently clear someone else's newer clipboard state).
- Vault corruption/tampering and failed-unlock tests; fail closed with clear recovery guidance.

## Later / evaluate by feedback

- Direct paste shortcuts for slots 1–9, subject to global hotkey conflicts and safety review.
- Window opacity / blur. Cosmetic and optional.
- Genia ecosystem landing page (GitHub Pages).
- Updated icon/branding and visible version label.
- AlternativeTo profile only after public release and authentic screenshots.
- LAN sync and team snippets only after demand and security design.
- Text Workbench: URL cleaning, casing, trim, JSON formatting, line transformations.

---

## Русский

GeniaClipboard развивается как **Clipboard Firewall + Encrypted Vault + Text Workbench**: локальный portable-менеджер для тех, кто в первую очередь работает с текстом. Это не попытка заменить все возможности Ditto или CopyQ.

**Пункты ниже — планы, а не готовые функции.**

### Сейчас — 0.5.6 beta

- Одинаковая безопасная вставка из истории, открытой хоткеем, треем или вручную.
- Отслеживание недавно активного внешнего окна, проверка окна и процесса, отказ от небезопасной вставки.
- Сброс выделения после скрытия в трей.
- Подготовка открытого тестирования, RU/EN документация, настоящие скриншоты.

### 0.5.7 — «О программе» и RU/EN интерфейс

- Раздел «О программе»: версия, лицензия MIT, GitHub, отчёты об ошибках, поддерживаемая Windows и честное описание ограничений защиты.
- Локализация через единый набор ресурсов (предпочтительно .resx): русский и английский.
- Автовыбор системного языка и ручной переключатель в «Настройки → Система».
- Перевести окна, трей, подсказки, ошибки, кнопки и сообщения. Текст истории и Vault не преобразовывать.

### 0.5.8 — удобство каждого дня

- Контекстное меню по ПКМ.
- Упрощённая нижняя панель.
- Маленькие индикаторы типов текст/URL/код/секрет (с эвристикой).
- Always on Top, редактор в панели предпросмотра.
- Подсветка поиска и группы дат («Сегодня», «Вчера», «На прошлой неделе»).

### 0.6.x — изображения, файлы и HTML

- Изображения с миниатюрами, ограничениями размера и зашифрованным хранением.
- Файлы Проводника; никогда автоматически не запускать скопированные файлы.
- HTML/RTF и fallback в обычный текст.
- Отдельная квота для бинарных данных и тесты миграции Vault.

### Безопасность и переносимость

- SHA-256 для EXE и ZIP.
- Импорт/экспорт зашифрованного Portable Vault.
- Опциональная автоблокировка 5/15/30/60 минут и безопасное удаление ключей из памяти.
- Опциональная очистка буфера при выходе с проверкой clipboard sequence number.
- Проверка повреждений Vault и понятная инструкция восстановления.

### Продвижение и экспериментальные функции

- Иконка, отображение версии, GitHub Pages.
- AlternativeTo и публикации на Хабре/Reddit после подтверждённого публичного релиза.
- Прямые хоткеи для первых записей, прозрачность, расширенные обработки текста — после отзывов пользователей.
