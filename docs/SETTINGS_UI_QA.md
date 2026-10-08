# GeniaClipboard 0.5.1 — settings UI manual QA

Windows x64 / WinForms. Automated CI verifies compilation and publish; visual layout needs interactive Windows validation.

## Layout / DPI

- [ ] Open Settings at 100%, 125%, and 150% Windows display scaling.
- [ ] Confirm four tabs: **Безопасность**, **История**, **Горячие клавиши**, **Система**.
- [ ] At the minimum window size, verify text and labels remain readable or can be reached by vertical scrolling.
- [ ] Verify **Сохранить** and **Отмена** stay visible while scrolling every tab.
- [ ] Ensure there is no horizontal scroll bar in the normal layout and no content hidden behind the footer.
- [ ] Vault selection values are readable; explanation appears below the selection.
- [ ] The process-exclusion editor is wide and accepts multiple lines.
- [ ] The red TXT-journal plaintext warning remains readable on the System tab.

## Interaction / compatibility

- [ ] Open/cancel Settings: stored values are unchanged.
- [ ] Change history limit, retention, sensitive expiry and clipboard clear; save/reopen and verify each value.
- [ ] Change the global shortcut, verify registration; attempt an occupied shortcut and verify the old shortcut remains.
- [ ] Toggle optional autostart and TXT journal.
- [ ] Change vault selection from Windows to Portable and back; password fields enable/disable correctly.
- [ ] For an invalid or mismatched Portable password, **Save** returns to the Security tab and does not apply changes.
- [ ] With all hotkey modifiers unchecked, **Save** returns to the Hotkeys tab.
- [ ] Verify upgrade from 0.5.0 preserves existing encrypted history and settings; back up the Data directory first.
- [ ] Double-click on headings, field labels, and warning text: clipboard content must not change.
- [ ] Verify main window, tray, capture rules, Private Session and TXT export remain operational.

No change to the vault encryption format or clipboard-capture policy is intended in 0.5.1.
