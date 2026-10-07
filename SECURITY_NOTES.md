# Security notes — GeniaClipboard 0.5.0

Updated 7 October 2026.

## Cryptographic storage

Persistent clipboard history is stored in `Data/history.gch`.

The file has a versioned binary envelope containing:

- format magic/version;
- vault mode;
- Portable Vault salt when applicable;
- 96-bit AES-GCM nonce;
- 128-bit authentication tag;
- ciphertext length;
- ciphertext.

The envelope mode and salt are included as AES-GCM additional authenticated data.

### Windows Vault

- random 256-bit AES key;
- key protected by Windows DPAPI with UI forbidden;
- protected key stored separately as `Data/history.key`;
- DPAPI scope is the current Windows user.

### Portable Vault

- random 128-bit salt;
- PBKDF2-HMAC-SHA256;
- 600,000 iterations;
- 256-bit derived AES key;
- master password is not persisted.

Keys held by `HistoryStore` are cleared with `CryptographicOperations.ZeroMemory` when replaced/disposed where possible. .NET strings used for password UI are immutable, so they cannot be reliably zeroed; the password itself is not retained after key derivation.

## Legacy migration

For a first 0.5.0 run with `Data/history.json` but no encrypted vault:

1. validate legacy file size;
2. deserialize legacy entries;
3. encrypt to a temporary vault;
4. decrypt and deserialize that temporary vault as verification;
5. atomically move the encrypted file into place;
6. delete the legacy plaintext file.

If legacy deletion fails, the application reports a warning.

## Clipboard Firewall

- own-process clipboard updates are ignored;
- GeniaClipboard internal clipboard writes contain a private format marker;
- internal writes also request exclusion from Windows clipboard-history/monitor processing;
- Windows `ExcludeClipboardContentFromMonitorProcessing` is respected;
- `CanIncludeInClipboardHistory=false` is respected conservatively;
- configured source processes are ignored;
- source process/window metadata is recorded for accepted text;
- heuristic sensitive detection covers several high-confidence token/key patterns;
- sensitive entries can expire automatically;
- configurable clipboard auto-clear only clears if the clipboard sequence number still matches the captured item, preventing deletion of newer clipboard content.

## Private Session

Private Session clears the in-memory visible history and disables persistence for subsequent operations. The persistent encrypted vault is not modified. Leaving the mode reloads the encrypted vault and discards temporary items.

Automatic TXT journaling is skipped while Private Session is active.

## Plaintext outputs

The optional TXT journal and manual TXT export are intentionally plaintext interoperability features. 0.5.0 therefore:

- never journals entries marked sensitive;
- never journals Private Session entries;
- displays a warning before manual TXT export.

## Remaining risks

- malware in the same Windows user session may read the system clipboard or process memory;
- PBKDF2 security depends on master-password strength;
- DPAPI ties Windows Vault portability to the Windows user profile;
- sensitive detection can have false positives and false negatives;
- exported TXT files are outside vault protection;
- Authenticode signing is not yet configured.

## Build/release hardening

- no third-party `PackageReference`;
- GitHub Actions are pinned to exact commit SHAs;
- release ZIP includes MIT license and bilingual README files;
- release workflow publishes `SHA256SUMS.txt`;
- Windows x64 CI performs restore and self-contained publish before merge.
