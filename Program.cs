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

        using var context = new GeniaClipboardApplicationContext();
        Application.Run(context);
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
