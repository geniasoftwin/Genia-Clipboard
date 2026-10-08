# Changelog

All notable GeniaClipboard changes are documented in this file.

## [0.5.4] - 2026-10-08

### Fixed

- kept the search row at its full 34-pixel height while centering its placeholder and text in a natural-height borderless TextBox;
- replaced the edit control border with the native Panel border, avoiding the previous resize paint artifacts;
- preserved all vault, paste-target and settings behavior from 0.5.3.

## [0.5.3] - 2026-10-08

### Fixed

- search field uses its natural single-line height and is vertically centered in the search row;
- automatic paste never reuses a stale foreground window after manual activation, Alt+Tab, or opening from the tray;
- automatic paste checks the exact target window and its process, and offers a manual Ctrl+V fallback if Windows blocks foreground switching;
- the Edit Entry and Portable Vault password dialogs use the application's branded icon;
- reduced the leading history marker column from 44 to 22 pixels, using a single colored marker for pinned sensitive items.

### Compatibility

- clipboard history vault format, encryption, and settings schema remain unchanged.

## [0.5.2] - 2026-10-08

### UI fixes

- switched to a native Windows search-field border to avoid redraw artifacts near the header;
- placed action buttons and vault/status text on separate footer rows to prevent clipping;
- added very subtle horizontal separators between visible history entries, without vertical grid lines;
- compacted Security and History settings and automatically grew the settings window within the working-screen bounds where possible;
- clarified that example process names are not active exclusions;
- kept the encrypted vault format and settings schema unchanged.

## [0.5.1] - 2026-10-08

### Improved

- replaced the long, scroll-heavy settings dialog with four focused tabs: Security, History, Hotkeys and System;
- made each settings tab scroll independently, with Save/Cancel permanently visible;
- shortened Vault selection labels and moved explanations into dedicated help text;
- made process exclusions a full-width multiline editor;
- kept the plaintext TXT journal warning visible within the System tab;
- improved layout behavior for Windows display scaling and narrower windows;
- validation returns to the relevant settings tab when a hotkey or master password is invalid.

### Compatibility

- no changes to the encrypted vault format, clipboard capture rules, or existing settings fields.

## [0.5.0] - 2026-10-07

### Added

- MIT license;
- encrypted clipboard-history vault using AES-256-GCM;
- Windows Vault with a random key protected by Windows DPAPI for the current user;
- Portable Vault with a PBKDF2-HMAC-SHA256 derived key and master password;
- verified migration from legacy plaintext `Data/history.json`;
- Private Session with memory-only history;
- configurable global hotkey;
- configurable history limit and age-based retention;
- optional current-user Windows autostart;
- per-process clipboard capture exclusions;
- Windows clipboard privacy-marker handling;
- source-process metadata for clipboard entries;
- heuristic sensitive-data detection;
- configurable sensitive-entry expiry;
- configurable system-clipboard auto-clear;
- F2 entry editor;
- SHA-256 checksum file for release ZIPs.

### Security

- sensitive entries are excluded from the plaintext automatic TXT journal;
- Private Session never writes clipboard history or TXT journal entries to disk;
- manual TXT export now displays an explicit plaintext warning;
- GeniaClipboard clipboard writes request exclusion from Windows clipboard-history/monitor processing;
- master passwords are not persisted;
- vault files are authenticated so ciphertext/header tampering fails closed.

## [0.4.4] - 2026-08

### Fixed

- the build script no longer passes the MSBuild publish/output path through `-o`;
- builds from directories containing spaces are more robust;
- files produced by the standard `dotnet publish` output are copied to `dist` by the batch script.

## [0.4.3] - 2026-08

### Fixed

- first attempt to improve paths containing spaces by using `pushd` and relative paths.

## [0.4.2] - 2026-08

### Fixed

- `build-portable.cmd` switches the console to UTF-8 with `chcp 65001`;
- Russian build messages render correctly;
- the batch file uses UTF-8 without BOM and CRLF line endings.

## [0.4.1] - 2026-08

### Changed

- increased the height of the top header area;
- improved search field height and internal padding.

### Fixed

- double-clicking passive UI labels no longer overwrites the system clipboard or creates history entries.

## [0.4.0] - 2026-08

### Added

- multi-selection;
- batch copy and paste;
- TXT export for selected entries or complete history;
- automatic daily TXT journal;
- private clipboard marker for self-copy protection.

### Security

- limits for entry size, history size, batch copy, and preview;
- streamed TXT export;
- safer local JSON writes through a temporary file;
- foreground-window verification before sending `Ctrl+V`.
