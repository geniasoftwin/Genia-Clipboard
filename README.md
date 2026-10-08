<p align="right"><a href="README_RU.md">Русский</a> · <strong>English</strong></p>

<p align="center">
  <img src="Assets/GeniaClipboard.svg" alt="GeniaClipboard logo" width="96" height="96">
</p>

<h1 align="center">GeniaClipboard</h1>

<p align="center">
  <strong>A portable, local-first clipboard history manager for Windows.</strong><br>
  Encrypted history · Private Session · Clipboard Firewall · No account required
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml"><img alt="Windows CI" src="https://github.com/geniasoftwin/Genia-Clipboard/actions/workflows/build.yml/badge.svg"></a>
  <img alt="License: MIT" src="https://img.shields.io/badge/license-MIT-green">
  <img alt="Windows x64" src="https://img.shields.io/badge/Windows-10%2F11%20x64-0078D6">
  <img alt="Version" src="https://img.shields.io/badge/next-0.5.6%20beta-2563EB">
</p>

<p align="center">
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/releases"><strong>Download for Windows</strong></a> ·
  <a href="docs/PUBLIC_BETA_GUIDE.md">Getting started</a> ·
  <a href="https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose">Report a bug / Suggest a feature</a>
</p>

> **0.5.6 beta candidate:** unified Paste for hotkey and tray use is under testing in [PR #9](https://github.com/geniasoftwin/Genia-Clipboard/pull/9). The latest **public release** may still be 0.5.5. Please don't describe beta-only features as released until the PR is merged and tested on Windows.

## Why another clipboard manager?

Windows Win+V, Ditto and CopyQ already exist. GeniaClipboard doesn't try to match their extensive format or automation support. Its focus is a **small, transparent, portable tool for text**, where clipboard history is encrypted at rest and users can decide what should be retained.

| Privacy first | Daily workflow | Portable Windows build |
|---|---|---|
| Encrypted history, Private Session, source-app exclusions, clipboard privacy markers | Search, pinning, multi-select, editing, configurable shortcut | Self-contained x64 EXE, local files, no account, no cloud or telemetry |

## How to use

1. Download the [latest public release ZIP](https://github.com/geniasoftwin/Genia-Clipboard/releases) and unpack it into a writable folder.
2. Start `GeniaClipboard.exe`. Choose Windows Vault (DPAPI) or Portable Vault (master password).
3. Copy text in any app. Open GeniaClipboard with `Ctrl+Shift+V` (or your configured shortcut) or via the tray.
4. Select a history entry. **Copy** puts it on the system clipboard; **Paste** attempts to restore a recently focused external window and send `Ctrl+V`.
5. If Windows won't activate the intended window, use **Copy** and press `Ctrl+V` manually. Without a selected clip, Enter does not paste.

**Important:** the app cannot guarantee that the text cursor remains at the same location when focus changes. Always confirm the target before pasting sensitive content.

## Screenshots

We are organizing [authentic Windows screenshots and bilingual captions](docs/SCREENSHOTS.md) for the project gallery. PNG assets will be embedded here **after** sanitized files are uploaded to the repository. The current project logo above is the actual GeniaClipboard SVG icon.

## Clipboard Firewall & encrypted storage

- **Windows Vault:** AES-256-GCM history, with a random key protected by Windows DPAPI for the current user. No master password required; not portable to another Windows user account by copying the file alone.
- **Portable Vault:** AES-256-GCM history, with a key derived from your master password using PBKDF2-HMAC-SHA256. Losing the password means losing access to history.
- **Private Session:** keeps new history only in memory and does not write the automatic TXT journal.
- **Capture controls:** respects Windows clipboard privacy markers, supports app exclusions, source attribution, optional sensitive-entry expiry, retention limits and timed system-clipboard clearing.
- **Interoperability:** optional daily TXT journal and manual TXT export.

**Security limits:** Encryption protects history **on disk**. It does not protect against software already able to read the Windows clipboard or process memory. Sensitive-content detection is heuristic and can miss secrets. TXT exports/journals are plaintext; `Data/settings.json` is not encrypted and contains preferences only. Do not share your `Data` folder or vault files in issues.

Read [Security Policy](SECURITY.md), [Threat Model](THREAT_MODEL.md), and [Security Notes](SECURITY_NOTES.md) for the technical boundaries.

## Features and limits

**Included:** text-only history; search; pin/unpin; app attribution; multiselect via Ctrl/Shift; copy/paste batches; F2 editor; 20–100,000 unpinned-entry limit; age retention; global hotkey; autostart for current user; tray; UTF-8 TXT export.

**Not supported yet:** clipboard images, Explorer file lists, HTML/RTF preservation, cloud/LAN sync. The release binary is not Authenticode-signed. See [Roadmap](ROADMAP.md).

## Portable update and source build

For upgrades, exit via the tray and **back up the entire `Data` folder** before replacing the EXE. Preserve the master password for Portable Vault.

To build from source, install the .NET 8 SDK / Windows desktop development workload, then run:

```bat
build-portable.cmd
```

Or:

```powershell
dotnet restore GeniaClipboard.csproj
dotnet publish GeniaClipboard.csproj -c Release
```

Release assets include a portable ZIP, the standalone `GeniaClipboard.exe`, and `SHA256SUMS.txt` with SHA-256 values for **both** files. On Windows, you can verify the downloaded file using `Get-FileHash .\GeniaClipboard.exe -Algorithm SHA256` (or substitute the ZIP name) and compare it with the matching checksum line. The EXE is self-contained, so an additional .NET runtime installation isn't required on the target PC.

## Participate

This is a small independent project seeking practical feedback, **especially on focus and paste behavior**. [Open an issue](https://github.com/geniasoftwin/Genia-Clipboard/issues/new/choose) with your Windows version, GeniaClipboard version, steps to reproduce and expected vs actual behavior. Please use **fictional test text**, never secrets or private clipboard records.

See [Contributing](CONTRIBUTING.md), [Changelog](CHANGELOG.md), [Public Beta Guide](docs/PUBLIC_BETA_GUIDE.md) and [Roadmap](ROADMAP.md).

**License:** [MIT](LICENSE) · © 2026 GeniaSoftWin
