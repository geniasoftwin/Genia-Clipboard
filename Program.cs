using System.Threading;

namespace GeniaClipboard;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;

        using var singleInstanceMutex = new Mutex(
            initiallyOwned: true,
            name: @"Local\GeniaClipboard.SingleInstance",
            createdNew: out var isFirstInstance);

        if (!isFirstInstance)
        {
            MessageBox.Show(
                "GeniaClipboard уже запущен. Найдите его значок в системном трее.",
                "GeniaClipboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var settingsStore = new AppSettingsStore();
        var settings = settingsStore.Settings;
        var vaultPath = Path.Combine(AppContext.BaseDirectory, "Data", "history.gch");
        var startupMode = File.Exists(vaultPath)
            ? HistoryStore.ProbeVaultMode()
            : settings.PreferredVaultMode;

        HistoryStore? store = null;
        string? error = null;

        if (startupMode == VaultMode.Portable)
        {
            while (store is null)
            {
                using var unlock = new MasterPasswordForm();
                if (unlock.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (!HistoryStore.TryOpen(settings, unlock.Password, out store, out error))
                {
                    MessageBox.Show(
                        error ?? "Не удалось разблокировать Portable Vault.",
                        "GeniaClipboard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }
        else if (!HistoryStore.TryOpen(settings, null, out store, out error))
        {
            MessageBox.Show(
                error ?? "Не удалось открыть Windows Vault.",
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
                    store.MigrationWarning,
                    "GeniaClipboard — миграция",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            using var context = new GeniaClipboardApplicationContext(settingsStore, store);
            Application.Run(context);
        }

        singleInstanceMutex.ReleaseMutex();
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Произошла ошибка:\n\n{e.Exception.Message}",
            "GeniaClipboard",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
