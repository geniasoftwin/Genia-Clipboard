# GeniaClipboard Roadmap

[Русский ниже](#русский)

## Product direction

GeniaClipboard is being developed as a **Clipboard Firewall + Encrypted Vault + Text Workbench**, not as a feature-for-feature clone of Ditto or CopyQ.

### 0.5.x — Privacy Core

Goal: make clipboard history safe enough to keep enabled every day.

- encrypted persistent history;
- Windows Vault and Portable Vault;
- Private Session;
- configurable hotkey and retention;
- application exclusions;
- Windows clipboard privacy markers;
- sensitive-data expiry;
- clipboard auto-clear;
- source-application metadata;
- entry editing;
- security documentation and checksummed releases.

### 0.5.5+ — Text Workbench

Goal: turn stored text into something immediately useful.

Planned:
- paste as plain text;
- trim whitespace;
- UPPERCASE / lowercase / Sentence case / Title Case;
- remove empty lines;
- deduplicate and sort lines;
- clean tracking parameters from URLs;
- URL encode/decode;
- JSON pretty/minify;
- transformation preview without modifying the original clip.

### 0.6.x — Rich Clipboard

Goal: support the formats most Windows users actually copy.

Planned:
- images with previews and dimensions;
- file lists;
- HTML;
- RTF;
- type filters;
- storage quotas for binary clipboard content.

### Later

Only after clear user demand:
- trusted-device LAN sync with authenticated pairing and end-to-end encryption;
- team snippet packs;
- additional automation and transformation workflows.

---

## Русский

GeniaClipboard развивается как **Clipboard Firewall + Encrypted Vault + Text Workbench**, а не как попытка повторить Ditto или CopyQ функция-в-функцию.

### 0.5.x — Privacy Core

Цель: сделать историю clipboard достаточно безопасной для постоянной работы.

- encrypted history;
- Windows Vault и Portable Vault;
- Private Session;
- hotkey и retention;
- исключения приложений;
- privacy-маркеры Windows;
- sensitive auto-expire;
- автоочистка clipboard;
- приложение-источник;
- редактор записей;
- security-документация и checksummed releases.

### 0.5.5+ — Text Workbench

Цель: превратить сохранённый текст в рабочий инструмент.

План:
- Paste as plain text;
- trim;
- UPPER/lower/Sentence/Title Case;
- удаление пустых строк;
- дедупликация и сортировка строк;
- очистка tracking-параметров URL;
- URL encode/decode;
- JSON pretty/minify;
- preview преобразования без изменения оригинала.

### 0.6.x — Rich Clipboard

Цель: поддержать основные форматы Windows clipboard.

План:
- изображения с preview;
- списки файлов;
- HTML;
- RTF;
- фильтры типов;
- storage quota для бинарных данных.

### Позже

Только при подтверждённом спросе:
- LAN sync с authenticated pairing и end-to-end encryption;
- командные snippet packs;
- дополнительные автоматизации.
