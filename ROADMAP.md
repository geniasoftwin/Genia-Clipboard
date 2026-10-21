# GeniaClipboard Roadmap

[Русский ниже](#русский)

## Positioning / Product direction

**Clipboard Firewall + Encrypted Vault + Text Workbench** for Windows — a local-first, portable alternative for people who primarily work with text. GeniaClipboard is not a feature-for-feature Ditto or CopyQ clone.

This roadmap contains **plans, not shipped features**. Items must be independently tested and checked off before they appear in README feature claims.

## Shipped — 0.5.6 / 0.5.7

- v0.5.6: guarded Paste for hotkey, tray and manual focus switching, window/process validation, no-target fallback, and selection reset.
- v0.5.7: real Russian/English UI, auto language selection (ru → Russian, otherwise English), manual override after restart, and About with MIT/source/feedback links.
- Genuine English and Russian screenshots are published in [the gallery](docs/SCREENSHOTS.md). SHA-256 for both EXE and ZIP is part of release assets.
- Both releases have passed Windows CI; v0.5.7 passed the owner's five Windows beta checks.

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

### Уже реализовано — 0.5.6 / 0.5.7

- **0.5.6:** защищённая вставка через хоткей, трей и вручную, проверка целевого окна/процесса, сброс выделения после скрытия.
- **0.5.7:** «О программе», RU/EN интерфейс, автоматический выбор языка Windows (русский для ru, английский для остальных), ручной выбор с применением после перезапуска.
- Настоящие скриншоты на русском и английском, публичная документация и SHA-256 для EXE и ZIP. Windows CI и пять ручных тестов 0.5.7 пройдены.

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
