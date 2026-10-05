<p align="right"><a href="README_RU.md">Русский</a> · <strong>English</strong></p>

<p align="center">
  <img src="Assets/GeniaClipboard.svg" alt="GeniaClipboard" width="96" height="96">
</p>

<h1 align="center">GeniaClipboard</h1>

<p align="center">
  A lightweight, portable clipboard history manager for Windows 10/11.
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml"><img alt="Build" src="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml/badge.svg"></a>
  <img alt="Version" src="https://img.shields.io/badge/version-0.4.4-blue">
  <img alt="Platform" src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows11&logoColor=white">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white">
</p>

GeniaClipboard runs in the system tray, collects copied text and links, lets you search clipboard history quickly, select multiple entries, and copy or paste them as a single block. All application data stays local next to the executable.

> [!WARNING]
> Clipboard managers can capture passwords, tokens, personal information, and other sensitive text. History and TXT journals are stored locally **without encryption**. Pause capture from the tray menu before copying secrets.

## Features

- clipboard history for text and URLs;
- up to 500 regular entries plus pinned items;
- instant history search;
- global `Ctrl+Shift+V` shortcut;
- system tray operation;
- protection against GeniaClipboard copying its own data back into history;
- exact duplicate handling;
- multi-selection with `Ctrl`, `Shift`, and `Ctrl+A`;
- batch copy and paste for multiple entries;
- delete multiple selected entries;
- manual UTF-8 TXT export;
- optional daily TXT journal;
- local storage in the `Data` directory;
- portable self-contained single-EXE build for Windows x64.

## Quick start

Install the .NET 8 SDK or Visual Studio with the **.NET desktop development** workload, then run:

```bat
build-portable.cmd
```

After a successful build, the portable archive is created at:

```text
dist\GeniaClipboard-win-x64.zip
```

The archive contains a self-contained `GeniaClipboard.exe`; .NET does not need to be installed on the target PC.

You can also build from the command line:

```powershell
dotnet restore GeniaClipboard.csproj
dotnet publish GeniaClipboard.csproj -c Release
```

## Usage

1. Start GeniaClipboard. It remains available from the system tray.
2. Copy text or links normally with `Ctrl+C`.
3. Open clipboard history with `Ctrl+Shift+V`.
4. Search the history or select several entries with `Ctrl`/`Shift`.
5. Click **Copy** or press `Ctrl+C` to place the selected entries on the clipboard as one block.
6. Paste them into Notepad, an editor, or another application with the normal `Ctrl+V`.

For batch copy, entries are joined in their displayed order, one entry per line.

## TXT export and auto journal

**Export TXT** saves either multiple selected entries or the complete history. Export is streamed and written as UTF-8.

The tray menu also provides **Auto journal TXT**. When enabled, new unique external clipboard entries are appended to a daily file:

```text
Data\Journal\GeniaClipboard_YYYY-MM-DD.txt
```

Data copied by GeniaClipboard itself is not added to the journal again.

## Privacy

GeniaClipboard does not use a cloud service, the Windows registry, or an external database. History, settings, and journal files are stored locally in the application directory.

To remove stored data, use **Clear** and, when necessary, delete the `Data\Journal` directory.

## Current limitations

- text only; images and copied file lists are not supported yet;
- the global shortcut is currently fixed to `Ctrl+Shift+V`;
- startup registration is intentionally not enabled so the application stays portable;
- current target platform is Windows x64;
- local history and journal files are not encrypted.

## Development

GeniaClipboard is written in C# / WinForms and currently has no third-party NuGet package dependencies.

GitHub Actions verifies the Windows build on pushes and pull requests. Third-party GitHub Actions are pinned to exact commit SHAs.

See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines, [CHANGELOG.md](CHANGELOG.md) for release history, and [SECURITY.md](SECURITY.md) for the security model.

## Version

Current stable release for this development stage: **0.4.4**.

> A project license has not been selected yet, so a `LICENSE` file is intentionally not included.
