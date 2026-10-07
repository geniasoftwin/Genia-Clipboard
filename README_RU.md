<p align="right"><strong>Русский</strong> · <a href="README.md">English</a></p>

<p align="center">
  <img src="Assets/GeniaClipboard.svg" alt="GeniaClipboard" width="96" height="96">
</p>

<h1 align="center">GeniaClipboard</h1>

<p align="center">
  Privacy-first портативный менеджер буфера обмена для Windows 10/11.
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml"><img alt="Build" src="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml/badge.svg"></a>
  <img alt="Version" src="https://img.shields.io/badge/version-0.5.0-blue">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green">
  <img alt="Platform" src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows11&logoColor=white">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white">
</p>

GeniaClipboard — local-first история буфера обмена с принципом: **полезная история не должна превращаться в открытый архив всего, что вы копируете**.

Версия 0.5.0 добавляет основу **Clipboard Firewall**: зашифрованную историю, исключения приложений, поддержку privacy-маркеров Windows, автоудаление sensitive-записей, автоочистку системного буфера и Private Session только в памяти.

## Зачем GeniaClipboard?

Основной акцент:

- **local-first** — без аккаунта, облака и телеметрии;
- **portable** — self-contained Windows x64;
- **зашифрованная история** — AES-256-GCM;
- **два режима vault** — Windows Vault и Portable Vault;
- **Private Session** — временная история без записи на диск;
- **Clipboard Firewall** — понятные правила, что нельзя сохранять;
- **простой интерфейс** — основные privacy-настройки без скриптов.

## Clipboard Firewall

0.5.0 умеет:

- игнорировать копирование самой GeniaClipboard;
- уважать privacy-маркеры Windows clipboard;
- не сохранять данные из выбранных пользователем процессов;
- распознавать ряд high-confidence секретов: заголовки private key, JWT, GitHub-style tokens, AWS access keys, bearer tokens и явные присваивания password/API key;
- автоматически удалять sensitive-записи через заданное время;
- очищать текущий системный clipboard через заданную задержку;
- сохранять источник принятой записи;
- не писать sensitive-записи в опциональный незашифрованный TXT-журнал.

Распознавание секретов эвристическое: оно не заменяет антивирус и не гарантирует обнаружение каждого секрета.

## Зашифрованный vault

История хранится в:

```text
Data\history.gch
```

вместо открытого `history.json`.

### Windows Vault

Режим по умолчанию.

- история: AES-256-GCM;
- случайный 256-битный ключ;
- ключ защищён Windows DPAPI для текущего Windows-пользователя;
- мастер-пароль не нужен.

Это удобно для основного ПК, но такой vault намеренно привязан к конкретному Windows-профилю.

### Portable Vault

Для переноса GeniaClipboard между компьютерами.

- история: AES-256-GCM;
- ключ выводится из мастер-пароля через PBKDF2-HMAC-SHA256;
- мастер-пароль нигде не сохраняется;
- во время работы в памяти остаётся только производный ключ.

Если мастер-пароль Portable Vault потерян, восстановить историю невозможно.

### Миграция с 0.4.x

Если 0.5.0 находит старый `Data\history.json`, а encrypted vault ещё отсутствует, программа:

1. читает и проверяет старую историю;
2. создаёт encrypted vault;
3. расшифровывает и повторно парсит новый vault для проверки;
4. только после этого удаляет старый plaintext-файл.

Если старый файл удалить нельзя, GeniaClipboard показывает предупреждение.

## Private Session

Private Session временно скрывает постоянный vault и начинает с пустой истории в оперативной памяти.

В этом режиме:

- новые записи не сохраняются на диск;
- TXT-журнал не записывается;
- выход из Private Session уничтожает временную историю;
- закрытие GeniaClipboard также уничтожает её.

Постоянный encrypted vault при этом не изменяется.

## Остальные возможности

- история текста и URL;
- настраиваемый лимит от 20 до 100 000 незакреплённых записей;
- необязательное удаление истории по возрасту;
- поиск по содержимому и приложению-источнику;
- переназначаемый глобальный хоткей;
- опциональный автозапуск для текущего Windows-пользователя;
- системный трей;
- закрепление;
- multi-select через `Ctrl`, `Shift`, `Ctrl+A`;
- пакетное копирование и вставка;
- редактор записи по `F2`;
- ручной UTF-8 TXT-экспорт;
- опциональный ежедневный TXT-журнал.

## Важные замечания о безопасности

Шифрование защищает сохранённую историю **на диске**. Оно не защищает clipboard или память процесса от вредоносной программы, уже работающей с достаточными правами в той же Windows-сессии.

Намеренно остаются незашифрованными:

- `Data\settings.json` — настройки приложения; ключи vault и мастер-пароль туда не записываются;
- автоматический TXT-журнал;
- ручной TXT-экспорт.

Перед ручным экспортом программа показывает предупреждение. Sensitive-записи и Private Session автоматически в TXT-журнал не попадают.

## Ограничения

- пока только текст: изображения, файлы, HTML и RTF запланированы на этап Rich Clipboard;
- sensitive detector эвристический;
- текущая платформа — Windows x64;
- релизные бинарники пока не имеют Authenticode-подписи.

## Разработка

Проект написан на C# / WinForms без сторонних NuGet-зависимостей.

GitHub Actions проверяет Windows-сборку на push и pull request. Сторонние Actions зафиксированы на конкретных commit SHA. Для релизного ZIP публикуется отдельный SHA-256 checksum.

См. [CONTRIBUTING_RU.md](CONTRIBUTING_RU.md), [CHANGELOG.md](CHANGELOG.md) и [SECURITY_RU.md](SECURITY_RU.md).

## Лицензия

GeniaClipboard распространяется под [MIT License](LICENSE).

## Версия

Текущая ветка релиза: **0.5.0 — Privacy Core**.
