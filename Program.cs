using System.Threading;

namespace GeniaClipboard;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        UiText.Configure("auto");
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;

        // An auto-reset activation request survives the startup/unlock phase.
        // A second launch signals the running UI instead of opening a dead-end dialog.
        using var activationEvent = new EventWaitHandle(
            initialState: false,
            mode: EventResetMode.AutoReset,
            name: @"Local\GeniaClipboard.Activate");

        using var singleInstanceMutex = new Mutex(
            initiallyOwned: true,
            name: @"Local\GeniaClipboard.SingleInstance",
            createdNew: out var isFirstInstance);

        if (!isFirstInstance)
        {
            activationEvent.Set();
            return;
        }

        var settingsStore = new AppSettingsStore();
        var settings = settingsStore.Settings;
        UiText.Configure(settings.Language);
        var vaultPath = Path.Combine(AppContext.BaseDirectory, "Data", "history.gch");
        var startupMode = File.Exists(vaultPath)
            ? HistoryStore.ProbeVaultMode()
            : settings.PreferredVaultMode;

        HistoryStore? store = null;
        string? error = null;

        if (settings.PrivateSessionOnStart)
        {
            store = HistoryStore.CreateLockedPrivateSession(settings, startupMode);
        }
        else if (startupMode == VaultMode.Portable)
        {
            while (store is null)
            {
                using var unlock = new MasterPasswordForm();
                var result = unlock.ShowDialog();

                if (result == DialogResult.Ignore)
                {
                    store = HistoryStore.CreateLockedPrivateSession(settings, startupMode);
                    break;
                }

                if (result != DialogResult.OK)
                {
                    return;
                }

                if (!HistoryStore.TryOpen(settings, unlock.Password, out store, out error))
                {
                    MessageBox.Show(
                        error is null ? UiText.T("Не удалось разблокировать Portable Vault.") : UiText.Error(error),
                        "GeniaClipboard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }
        else if (!HistoryStore.TryOpen(settings, null, out store, out error))
        {
            MessageBox.Show(
                error is null ? UiText.T("Не удалось открыть Windows Vault.") : UiText.Error(error),
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        using (store)
        {
            if (settingsStore.LastError is not null)
            {
                MessageBox.Show(
                    settingsStore.LastError,
                    "GeniaClipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            if (store.MigrationWarning is not null)
            {
                MessageBox.Show(
                    UiText.Error(store.MigrationWarning),
                    UiText.T("GeniaClipboard — миграция"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            var startInTray = args.Any(arg => string.Equals(arg, "--tray", StringComparison.OrdinalIgnoreCase));
            using var context = new GeniaClipboardApplicationContext(
                settingsStore, store, activationEvent, startInTray);
            Application.Run(context);
        }

        singleInstanceMutex.ReleaseMutex();
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        MessageBox.Show(
            (UiText.IsEnglish ? $"An error occurred:\n\n{e.Exception.Message}" : $"Произошла ошибка:\n\n{e.Exception.Message}"),
            "GeniaClipboard",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
