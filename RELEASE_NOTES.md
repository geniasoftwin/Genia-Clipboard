# GeniaClipboard 0.5.0 — Privacy Core

[Русский ниже](#русский)

## English

0.5.0 turns GeniaClipboard into a privacy-first clipboard vault rather than a plaintext clipboard archive.

Highlights:
- MIT licensed;
- AES-256-GCM encrypted history;
- Windows Vault protected by Windows DPAPI;
- Portable Vault protected by a master-password-derived key;
- safe verified migration from 0.4.x plaintext history;
- memory-only Private Session;
- configurable hotkey, history limit, retention, and Windows autostart;
- application exclusions and Windows clipboard privacy markers;
- source-process metadata;
- sensitive-data detection and auto-expiry;
- optional system-clipboard auto-clear;
- F2 entry editing;
- release ZIP accompanied by SHA-256 checksums.

Security boundary: encryption protects persisted history at rest. It does not protect clipboard contents or process memory from malware already running with sufficient access in the same Windows session.

TXT exports and the optional TXT journal remain plaintext by design. Sensitive entries and Private Session are never written to the automatic journal.

## Русский

0.5.0 превращает GeniaClipboard из plaintext-архива буфера в privacy-first clipboard vault.

Главное:
- лицензия MIT;
- история зашифрована AES-256-GCM;
- Windows Vault с ключом под защитой Windows DPAPI;
- Portable Vault с ключом из мастер-пароля;
- проверяемая миграция старого `history.json`;
- Private Session только в оперативной памяти;
- настраиваемый хоткей, лимит/срок истории и автозапуск Windows;
- исключения приложений и privacy-маркеры Windows clipboard;
- сохранение приложения-источника;
- sensitive detector и автоудаление sensitive-записей;
- опциональная автоочистка системного clipboard;
- редактор записей по F2;
- отдельный SHA-256 checksum релизного ZIP.

Граница защиты: шифрование защищает сохранённую историю на диске, но не clipboard и память процесса от вредоносной программы, уже работающей с достаточными правами в той же Windows-сессии.

TXT-экспорт и опциональный TXT-журнал остаются plaintext. Sensitive-записи и Private Session в автоматический журнал не записываются.
