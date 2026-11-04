# GeniaClipboard v0.5.8-rc.1 — startup and window visibility QA

This checklist is for the **test branch only**, not the released v0.5.7 binary.

## Safety and setup

- Keep the released v0.5.7 EXE/ZIP as your rollback.
- **Never remove, replace, move, rename or upload your existing `Data` directory or Portable Vault.**
- Download the Windows CI artifact and extract the **test EXE into a new empty directory**, separate from your production GeniaClipboard directory.
- Quit any running GeniaClipboard via the tray before launching a test copy (single-instance mutex).
- The test copy may create its **own new `Data`** directory as part of normal operation. Do not copy any existing `Data` into this test directory.
- Use only dummy clipboard strings. Do not share vault passwords, vault files or clipboard history in diagnostics.

## Acceptance matrix

| ID | Action | Expected |
|---|---|---|
| S1 | Launch `GeniaClipboard.exe` normally in the isolated directory. Complete any first-run dialog. | Main history window is visible and can receive focus. |
| S2 | Close the main window with X. | Window hides; process remains running and tray icon is present. |
| S3 | Double-click the tray icon, then hide with Esc; right-click tray icon and select Open. | Both routes reliably restore the same window. |
| S4 | Press configured global hotkey (default Ctrl+Shift+V). | Main window opens. If a different program owns the hotkey, tray routes still open it and the status reports unavailable registration. |
| S5 | With the app running but hidden, launch the EXE again. | Already-running instance opens its window, without a second process or second vault instance. Allow up to one timer interval (700 ms) for activation. |
| S6 | Quit from tray. Run `GeniaClipboard.exe --tray` from the test folder. | Starts in tray with main window hidden; tray/hotkey still restore it. |
| S7 | From a hidden state use tray Settings, About, Private Session or Clear History (no deletion confirmation). | Related dialogs open in front of a visible main window. Cancel any destructive prompt. |
| S8 | After tests, quit from tray. | Process exits cleanly. Production v0.5.7 and its Data remain untouched. |

## Windows CI scope

The pull request's Windows build checks .NET 8 restore and self-contained x64 publish. **GitHub Actions cannot establish interactive tray/foreground behavior**; the S1–S8 acceptance steps require a real Windows desktop.

Do not merge to `main`, retag a release or overwrite production files until desktop acceptance passes.
