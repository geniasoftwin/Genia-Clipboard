# Changelog

All notable GeniaClipboard changes are documented in this file.

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
