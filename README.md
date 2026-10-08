<p align="right"><a href="README_RU.md">Русский</a> · <strong>English</strong></p>

<p align="center">
  <img src="Assets/GeniaClipboard.svg" alt="GeniaClipboard" width="96" height="96">
</p>

<h1 align="center">GeniaClipboard</h1>

<p align="center">
  Privacy-first, portable clipboard manager for Windows 10/11.
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml"><img alt="Build" src="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml/badge.svg"></a>
  <img alt="Version" src="https://img.shields.io/badge/version-0.5.6--beta-blue">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green">
  <img alt="Platform" src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows11&logoColor=white">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white">
</p>

GeniaClipboard is a local-first clipboard history tool built around a simple idea: **clipboard history should be useful without becoming a plaintext archive of everything you copy**.

Version 0.5.0 introduces the **Clipboard Firewall** privacy core: encrypted history, application exclusions, Windows clipboard privacy markers, sensitive-data expiry, clipboard auto-clear, a memory-only Private Session, and configurable behavior.

## Public beta testing — 0.5.6

**Open source:** [MIT License](LICENSE). Source is public, and contributions and issues are welcome.

**Try it:** [Windows x64 releases](https://github.com/geniasoftwin/Genia-Clipboard/releases) · [beta guide](docs/PUBLIC_BETA_GUIDE.md) · [report a bug or request a feature](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose).

**Unified Paste:** the most recently focused eligible external window is tracked regardless of whether history is opened with the hotkey, tray or manually. Select a clip and press **Paste** to copy it and attempt to restore the external window before Ctrl+V. Without an explicit selection, Enter never pastes.

**Safety limitation:** the app can track a recent external window but cannot guarantee where the user's text caret is. Confirm the intended destination before auto-pasting.

**Screenshots:** authentic sanitized captures of the main window, settings and editor are being prepared. We will add those instead of mock screenshots.

## Why GeniaClipboard?

GeniaClipboard is not trying to out-feature every mature clipboard manager. Its focus is:

- **local-first** — no account, no cloud service, no telemetry;
- **portable** — self-contained Windows x64 build;
- **encrypted history** — AES-256-GCM authenticated encryption at rest;
- **two vault modes** — Windows Vault or Portable Vault;
- **Private Session** — temporary clipboard history that is never written to disk;
- **Clipboard Firewall** — explicit rules for what should not be stored;
- **simple UI** — common privacy controls do not require scripting.

## Clipboard Firewall

0.5.0 can:

- ignore copies originating from GeniaClipboard itself;
- respect Windows clipboard privacy markers such as content excluded from clipboard-history processing;
- exclude user-selected source processes from capture;
- detect several high-confidence secret patterns such as private-key headers, JWTs, GitHub-style tokens, AWS access keys, bearer tokens, and explicit password/API-key assignments;
- auto-expire sensitive entries after a configurable time;
- auto-clear the current system clipboard after a configurable delay;
- record the source process for accepted clipboard entries;
- keep sensitive entries out of the optional plaintext TXT journal.

Sensitive detection is heuristic and is **not** a malware defense or a guarantee that every secret will be recognized.

## Encrypted vault

Clipboard history is stored in:

```text
Data\history.gch
```

rather than plaintext `history.json`.

### Windows Vault

The default mode.

- history payload: AES-256-GCM;
- random 256-bit vault key;
- vault key protected with Windows DPAPI for the current Windows user;
- no master password required.

This is convenient for a primary PC, but the vault is intentionally tied to that Windows user profile.

### Portable Vault

Designed for moving GeniaClipboard between PCs.

- history payload: AES-256-GCM;
- key derived from a user-provided master password with PBKDF2-HMAC-SHA256;
- master password is not stored;
- only the derived key remains in memory while the application is running.

A Portable Vault cannot be recovered if the master password is lost.

### Migration from 0.4.x

When 0.5.0 finds a legacy `Data\history.json` and no encrypted vault yet, it:

1. loads and validates the legacy history;
2. creates the encrypted vault;
3. decrypts and parses the new vault as a verification step;
4. only then deletes the old plaintext history.

If the legacy file cannot be removed, GeniaClipboard shows a warning instead of silently claiming the migration is complete.

## Private Session

Private Session temporarily hides the persistent vault and starts with an empty in-memory history.

While it is active:

- new entries are not written to disk;
- the TXT journal is not written;
- leaving Private Session discards its temporary history;
- exiting GeniaClipboard discards the temporary history.

The encrypted persistent vault is left untouched.

## Other features

- text and URL clipboard history;
- configurable history limit from 20 to 100,000 unpinned entries;
- optional age-based retention;
- search across text and source application;
- configurable global hotkey;
- optional current-user Windows autostart;
- system tray operation;
- pinning;
- multi-selection with `Ctrl`, `Shift`, and `Ctrl+A`;
- batch copy and paste;
- F2 entry editor;
- manual UTF-8 TXT export;
- optional daily TXT journal;
- local settings in the application directory.

## Quick start

Install the .NET 8 SDK or Visual Studio with the **.NET desktop development** workload, then run:

```bat
build-portable.cmd
```

After a successful build:

```text
dist\GeniaClipboard-win-x64.zip
```

The archive contains a self-contained `GeniaClipboard.exe`; .NET does not need to be installed on the target PC.

You can also build from the command line:

```powershell
dotnet restore GeniaClipboard.csproj
dotnet publish GeniaClipboard.csproj -c Release
```

## Important privacy notes

Encryption protects persisted history **at rest**. It does not protect clipboard contents or process memory from malware already running with sufficient access in the same user session.

The following remain intentionally unencrypted when enabled or explicitly created:

- `Data\settings.json` — contains application preferences, not vault keys or the Portable Vault master password;
- automatic TXT journal files;
- manual TXT exports.

GeniaClipboard warns before manual TXT export. Sensitive entries and Private Session are never written to the automatic TXT journal.

## Settings UI in 0.5.1

Settings now have four focused tabs: **Security**, **History**, **Hotkeys**, and **System** (tab names in the current UI are Russian). Save and Cancel remain visible in a fixed footer while each tab scrolls independently. Vault-mode descriptions, process exclusions, and plaintext journal warnings have more room, including at increased Windows display scaling.

## UI polish in 0.5.2

History rows have subtle horizontal separators; the search field uses native rendering to avoid redraw artifacts, and the main status has its own row. Security and History settings pages adjust the dialog height to the available screen and aim to open without scrollbars at normal display scaling. Small screens and very high DPI may still need scrolling.

Version 0.5.3 also reduces the pinned/sensitive marker column from 44 to 22 pixels so history text begins closer to the left edge.

## Search-field refinement in 0.5.4

The search row now keeps its full height, with the text and placeholder centered vertically inside a borderless edit control. A native Windows panel border replaces custom border painting. The Clipboard Firewall, vault format, hotkey paste behavior and settings schema are unchanged.

## Selection safety in 0.5.5

After a paste, GeniaClipboard still returns to the system tray. On the next hotkey invocation, **no clip is selected**: press the Down Arrow to choose the first visible entry, navigate with the arrows, or click entries with the mouse. Enter and Paste operate only on a deliberately selected clip. The old multi-selection is never reused automatically.

## Current limitations

- text only; images, copied file lists, HTML, and RTF are planned for the Rich Clipboard stage;
- sensitive-data detection is heuristic;
- Windows x64 is the current target;
- release binaries are not yet Authenticode-signed.

## Development and security

GeniaClipboard is written in C# / WinForms and currently has no third-party NuGet package dependencies.

GitHub Actions verifies the Windows build on pushes and pull requests. Third-party Actions are pinned to exact commit SHAs. Release ZIP files include a SHA-256 checksum file.

See [CONTRIBUTING.md](CONTRIBUTING.md), [CHANGELOG.md](CHANGELOG.md), and [SECURITY.md](SECURITY.md).

## License

GeniaClipboard is released under the [MIT License](LICENSE).

## Version

Development branch: **0.5.6 — Unified Paste (beta)**. Published builds are available under Releases.
