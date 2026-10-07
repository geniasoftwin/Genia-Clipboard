# Security Policy

**English** · [Русский](SECURITY_RU.md)

## Supported version

The current supported release line is `0.5.x`.

## Security model

GeniaClipboard is a local clipboard manager. Clipboard contents are inherently sensitive and are visible to software running in the same Windows user session.

0.5.0 protects persisted history with authenticated encryption:

- history payloads use AES-256-GCM;
- Windows Vault uses a random 256-bit key protected with Windows DPAPI for the current user;
- Portable Vault derives a 256-bit key from a master password using PBKDF2-HMAC-SHA256;
- the Portable Vault master password is not written to disk;
- encrypted history is authenticated and corrupted/tampered ciphertext fails closed.

Encryption protects data **at rest**. It is not a defense against malware already running with sufficient access to the same Windows user session, process memory, or system clipboard.

## Clipboard Firewall

GeniaClipboard can:

- respect Windows clipboard privacy markers;
- exclude configured source processes;
- keep its own clipboard writes out of its history;
- heuristically detect several high-confidence secret patterns;
- auto-expire sensitive entries;
- auto-clear the system clipboard after a configured delay;
- run in a memory-only Private Session.

Sensitive detection is best-effort and must not be treated as a guarantee.

## Plaintext outputs

These are intentionally not encrypted:

- `Data/settings.json` — preferences only; no vault key or master password;
- optional TXT journal files;
- manual TXT exports.

Sensitive entries and Private Session are not written to the automatic TXT journal. Manual export requires explicit user confirmation.

## Reporting a vulnerability

Do not post real passwords, tokens, personal information, or clipboard contents in a public issue.

For non-sensitive reports, open an issue with a minimal reproducible example. For reports containing sensitive technical details, contact the repository owner through GitHub first and do not attach real secrets.
