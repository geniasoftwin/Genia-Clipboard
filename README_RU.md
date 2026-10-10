<p align="right"><strong>Русский</strong> · <a href="README.md">English</a></p>

<p align="center">
  <img src="Assets/GeniaClipboard.svg" alt="Логотип GeniaClipboard" width="96" height="96">
</p>

<h1 align="center">GeniaClipboard</h1>

<p align="center">
  <strong>Портативный менеджер истории буфера обмена для Windows.</strong><br>
  Зашифрованная история · Private Session · Clipboard Firewall · Без аккаунта
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml"><img alt="Windows CI" src="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml/badge.svg"></a>
  <img alt="MIT License" src="https://img.shields.io/badge/license-MIT-green">
  <img alt="Windows x64" src="https://img.shields.io/badge/Windows-10%2F11%20x64-0078D6">
  <img alt="Beta" src="https://img.shields.io/badge/version-0.5.6%20beta-2563EB">
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/releases"><strong>Скачать для Windows</strong></a> ·
  <a href="docs/PUBLIC_BETA_GUIDE.md">Инструкция</a> ·
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose">Сообщить об ошибке / Предложить функцию</a>
</p>

> **0.5.6 — публичная бета:** автор проверил на Windows сценарий «Копировать» → перевод фокуса в Блокнот → возвращение в GeniaClipboard → «Вставить». В других приложениях или при иных переключениях окон поведение может отличаться. Сообщайте о воспроизводимых ошибках.

## Зачем ещё один менеджер буфера обмена?

В Windows уже есть Win+V, а Ditto и CopyQ гораздо богаче возможностями. GeniaClipboard не пытается повторить все их форматы и автоматизацию. Наша ниша — **простой портативный инструмент для текста**, где история шифруется на диске, а пользователь контролирует, какие записи сохранять.

| Конфиденциальность | Ежедневная работа | Портативность |
|---|---|---|
| Зашифрованный Vault, Private Session, исключения приложений, privacy-маркеры Windows | Поиск, закрепление, множественное выделение, редактирование, хоткей | Самодостаточный EXE для Windows x64, локальные файлы, без облака и телеметрии |

## Как пользоваться

1. Скачайте [ZIP последнего публичного релиза](https://github.com/geniasoftwin/Genia-Clipboard/releases) и распакуйте в доступную для записи папку.
2. Запустите `GeniaClipboard.exe`. Выберите Windows Vault (DPAPI) или Portable Vault (с мастер-паролем).
3. Копируйте текст из разных приложений. Откройте историю `Ctrl+Shift+V` (или настроенным хоткеем) либо через трей.
4. **Явно выделите запись.** «Копировать» помещает её в системный буфер; «Вставить» пытается вернуть фокус в недавно активное внешнее окно и отправить `Ctrl+V`.
5. Если Windows не позволяет активировать целевое окно, используйте «Копировать» и ручное `Ctrl+V`. Без выделенной строки Enter ничего не вставляет.

**Важное ограничение:** программа не может гарантировать, что курсор ввода остался в прежнем месте после смены окон. Перед вставкой чувствительных данных проверьте целевое окно.

## Скриншоты — настоящий интерфейс Windows

<sub>Ниже — настоящие снимки публичной версии **0.5.6 на русском**. Уже доступны и [новые английские скриншоты тестовой версии 0.5.7](docs/SCREENSHOTS.md#english-v057-beta--english-interface), но сам релиз 0.5.7 пока не опубликован. Все кадры сделаны в реальной программе, без наложенного перевода.</sub>

<p align="center">
  <img src="docs/screenshots/01-main-history.png" width="806" alt="GeniaClipboard — главное окно и история буфера обмена">
</p>

<details>
<summary><strong>Показать ещё: поиск, безопасность и история</strong></summary>

**Поиск и фильтрация записей**

![Поиск по истории GeniaClipboard](docs/screenshots/03-search-filter.png)

**Зашифрованный Vault и настройки защиты**

![Настройки безопасности GeniaClipboard](docs/screenshots/04-settings-security.png)

**Хранение истории и исключения приложений**

![Настройки истории GeniaClipboard](docs/screenshots/05-settings-history.png)

</details>

[Посмотреть все 9 скриншотов с подписями](docs/SCREENSHOTS.md).

## Clipboard Firewall и зашифрованная история

- **Windows Vault:** AES-256-GCM, случайный ключ под защитой Windows DPAPI текущего пользователя. Пароль не требуется; простое копирование файла не переносит доступ между пользователями Windows.
- **Portable Vault:** AES-256-GCM, ключ из мастер-пароля через PBKDF2-HMAC-SHA256. Если забыть пароль, расшифровать историю не получится.
- **Private Session:** временные записи живут в оперативной памяти, не пишутся в Vault и автоматический TXT-журнал.
- **Правила захвата:** privacy-маркеры Windows, исключения процессов, запись приложения-источника, таймеры удаления sensitive-записей, лимит и срок истории, опциональная очистка системного буфера.
- **Экспорт:** ручной TXT-экспорт и опциональный ежедневный TXT-журнал.

**Границы безопасности:** шифрование защищает историю **на диске**, но не от ПО, уже имеющего доступ к системному буферу или памяти процесса. Sensitive detector — эвристический, может пропускать секреты. TXT-журнал и TXT-экспорт — **незашифрованные**. `Data/settings.json` содержит только настройки и также не шифруется. Не прикладывайте папку `Data` или файлы Vault к публичным issue.

Подробности: [Security Policy](SECURITY_RU.md), [Threat Model](THREAT_MODEL.md), [Security Notes](SECURITY_NOTES.md).

## Возможности и ограничения

**Есть:** история текстовых записей; поиск; закрепление; приложение-источник; выделение Ctrl/Shift; пакетное копирование/вставка; редактор F2; лимит 20–100 000 незакреплённых записей; срок хранения; настраиваемый хоткей; автозапуск текущего пользователя; трей; UTF-8 TXT-экспорт.

**Пока нет:** изображений, файлов Проводника, сохранения форматов HTML/RTF и синхронизации. EXE пока не подписан Authenticode. См. [Roadmap](ROADMAP.md).

## Обновление и сборка из исходников

Перед обновлением выйдите через трей и сохраните **резервную копию всей папки `Data`**, затем заменяйте EXE. Для Portable Vault не потеряйте мастер-пароль.

Для самостоятельной сборки на Windows потребуется .NET 8 SDK и компоненты Windows desktop development:

```bat
build-portable.cmd
```

Либо:

```powershell
dotnet restore GeniaClipboard.csproj
dotnet publish GeniaClipboard.csproj -c Release
```

К релизу прилагаются portable ZIP, отдельный `GeniaClipboard.exe` и `SHA256SUMS.txt` с SHA-256 **обоих** файлов. Для проверки в PowerShell выполните `Get-FileHash .\GeniaClipboard.exe -Algorithm SHA256` (для архива подставьте имя ZIP) и сравните результат с соответствующей строкой в `SHA256SUMS.txt`. EXE автономный: отдельно устанавливать .NET runtime на целевой ПК не нужно.

## Обратная связь и участие

Это небольшой независимый проект, которому важны практические отзывы, особенно о **переключении окон и вставке**. [Создайте GitHub Issue](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose) с версией Windows, версией программы, шагами, ожидаемым и фактическим поведением. Используйте **только вымышленные тестовые строки**, без настоящих секретов.

См. [Участие в разработке](CONTRIBUTING_RU.md), [История изменений](CHANGELOG.md), [Инструкция beta-тестера](docs/PUBLIC_BETA_GUIDE.md) и [План развития](ROADMAP.md).

**Лицензия:** [MIT](LICENSE) · © 2026 GeniaSoftWin
