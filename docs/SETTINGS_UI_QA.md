# GeniaClipboard 0.5.2 — Windows UI regression checklist

GitHub CI verifies compile and self-contained publish. Visual behavior must also be checked manually on Windows.

## Main window

- [ ] Copy several lines as individual clips; each visible row has a very subtle horizontal separator and no vertical gridlines.
- [ ] Selected, pinned and sensitive row backgrounds remain readable; scrolling does not leave separator traces.
- [ ] **Вставить** is blue when enabled, muted when disabled, and has a distinguishable hover state.
- [ ] **Очистить** is red; it still asks for confirmation before deleting history.
- [ ] Copy/Edit/Delete/Export/Settings remain neutral.
- [ ] Resize/restore at 100%, 125% and 150% DPI; the search border remains continuous without vertical colored ticks.
- [ ] With a narrow main window, the Vault/status text is visible on its own row; button row can scroll horizontally if needed.

## Settings

- [ ] On an ordinary desktop at default scale, Security and History open without a vertical scrollbar.
- [ ] On small screens or at 150% DPI, controls can be reached via scrolling and do not overlap the fixed Save/Cancel footer.
- [ ] The window remains within the current monitor's working area.
- [ ] No active process exclusions are prepopulated; examples are visibly described as examples.
- [ ] Vault, auto-start, hotkey and sensitive-data settings preserve their values after Save/Cancel.

## Safety

- [ ] Upgrade from 0.5.1 with a backed-up Data directory; encrypted vault and settings load unchanged.
- [ ] Double-clicking passive labels does not write to the clipboard.
- [ ] A new clipboard entry is captured normally, while an internal GeniaClipboard copy is not duplicated.
- [ ] Private Session and TXT export behavior remains unchanged.

This is a UI-only release: the encrypted vault format and settings schema have not changed.
