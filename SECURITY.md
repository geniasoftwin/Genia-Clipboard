# Security Policy

**English** · [Русский](SECURITY_RU.md)

## Supported version

At this stage, the latest published `0.4.x` release is supported. The current stable version is `0.4.4`.

## Security model

GeniaClipboard is a local clipboard manager and therefore handles potentially sensitive data by design.

- `Data/history.json`, `Data/settings.json`, and TXT journal files are stored locally **without encryption**;
- processes running in the same Windows user session may in principle read or modify the system clipboard;
- GeniaClipboard's private clipboard format prevents self-duplication but is not a security boundary;
- the application cannot reliably determine whether copied text is a password, token, API key, or other secret.

Pause capture and disable the auto journal before copying sensitive information.

## Defensive measures

The current version includes limits for clipboard entry and history sizes, streamed TXT export, safer local JSON replacement, foreground-window verification before automatic paste, no third-party `PackageReference` dependencies, and GitHub Actions pinned to exact commit SHAs.

Additional technical details are documented in `SECURITY_NOTES.md`.

## Reporting a vulnerability

Do not post real passwords, tokens, personal information, or clipboard contents in a public issue.

Non-sensitive reports may be submitted as an issue with a minimal reproducible example. If a report contains sensitive technical details, contact the repository owner through GitHub first and do not attach real secrets.
