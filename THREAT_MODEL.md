# GeniaClipboard Threat Model

This document states what GeniaClipboard 0.5.x is designed to protect and what it is not designed to protect.

## Protected scenarios

### Stolen/copied history files

An attacker who obtains `Data/history.gch` alone should not be able to read clipboard history.

For Windows Vault, `history.key` is DPAPI-protected for the current Windows user.

For Portable Vault, the encryption key is derived from the user's master password and is not stored directly.

### Accidental plaintext persistence

Legacy plaintext history is migrated only after the encrypted replacement is successfully decrypted and parsed.

Private Session does not persist captured history.

Sensitive entries are not written to the automatic TXT journal.

### Accidental clipboard retention

Optional clipboard auto-clear uses the Windows clipboard sequence number and only clears the exact clipboard state it scheduled. A newer clipboard item is not deleted by an old timer.

### Applications requesting privacy

GeniaClipboard respects Windows clipboard privacy markers that request exclusion from clipboard-history processing.

## Out of scope

GeniaClipboard does not claim to protect against:

- malware running as the same user with access to the Windows clipboard;
- malware reading GeniaClipboard process memory;
- an attacker who knows the Portable Vault master password;
- an attacker controlling an already-unlocked Windows account;
- screen capture, keylogging, accessibility APIs, or kernel-level compromise;
- plaintext files created deliberately through manual TXT export.

## Trust boundaries

Process-name exclusions are a convenience/privacy policy, not an authentication mechanism. A malicious process can imitate another executable name.

Sensitive-data detection is heuristic and can produce false positives or false negatives.

Windows Vault is intentionally bound to the Windows user profile. Portable Vault is the mode intended for moving encrypted history between PCs.
