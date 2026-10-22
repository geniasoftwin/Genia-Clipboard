# GeniaClipboard v0.5.7.1 — Window activation hotfix (test candidate)

[Русский ниже](#русский)

## English

This is a small reliability fix on top of v0.5.7 (RU/EN UI). It does not change encrypted history, existing vault passwords, clipboard content, or the guarded Paste target validation.

- **Manual launch:** Open the main clipboard history window immediately after unlocking the Vault, instead of silently launching into the tray.
- **Windows autostart:** The Run registry entry uses `--minimized` so signing in starts quietly in the system tray as before.
- **Second EXE launch:** Signals the existing running instance to show its main window, even if the first instance is hidden in the tray.
- **About:** Displays the full hotfix version `0.5.7.1`.

### Feedback and testing required

Before release, test:
1. Exit the application via the tray; run the EXE manually. The window should appear.
2. Close the window using X: the application should remain available in the tray.
3. Double-click the EXE again while GeniaClipboard is already running: the existing window should reappear and no duplicate process should run.
4. Open it using Ctrl+Shift+V and the tray; the guarded Paste into Notepad must continue working.
5. When Windows autostart is enabled, sign in again and confirm it starts in the tray without opening the window. (After installation or first launch, the autostart entry is refreshed to pass `--minimized`.)
6. Verify the previously encrypted Vault unlocks, and Settings → System → About shows `0.5.7.1`.

The EXE is not Authenticode-signed. Prior to upgrading, exit through the tray and **back up the entire `Data` directory**. Portable Vault users must preserve their master password.

## Русский

Небольшое исправление запуска поверх v0.5.7 (RU/EN интерфейс). **История, шифрование Vault, мастер-пароли, содержимое буфера и защищённая вставка не изменены.**

- **Обычный запуск EXE:** после разблокировки Vault сразу показывается главное окно вместо незаметного запуска в трей.
- **Автозапуск Windows:** в записи автозапуска используется `--minimized` — при входе Windows программа по-прежнему тихо запускается в трее.
- **Повторный запуск EXE:** уже работающая копия получает сигнал и раскрывает главное окно из трея; второй экземпляр не запускается.
- **«О программе»:** показывает точную версию `0.5.7.1`.

### Что проверить перед выпуском

1. Выйти через трей и запустить EXE: главное окно должно появиться.
2. Нажать X: окно скрывается в трей, программа продолжает работу.
3. Повторно запустить EXE: открывается окно существующей копии без второго процесса.
4. Проверить Ctrl+Shift+V, трей и вставку из истории в Блокнот.
5. С включённым автозапуском проверить следующий вход в Windows: старт только в трее.
6. Убедиться, что Portable Vault/Windows Vault и вся история не изменились, а в About указано `0.5.7.1`.

EXE не подписан Authenticode. **Перед заменой EXE выйдите через трей и сохраните копию всей папки `Data`.** Не теряйте пароль Portable Vault.
