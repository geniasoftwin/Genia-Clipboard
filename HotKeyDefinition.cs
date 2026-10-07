namespace GeniaClipboard;

internal readonly record struct HotKeyDefinition(uint Modifiers, Keys Key)
{
    public static HotKeyDefinition FromSettings(AppSettings settings)
    {
        return new HotKeyDefinition(settings.HotKeyModifiers, (Keys)settings.HotKeyKey);
    }

    public string ToDisplayString()
    {
        var parts = new List<string>(5);

        if ((Modifiers & NativeMethods.ModControl) != 0)
        {
            parts.Add("Ctrl");
        }

        if ((Modifiers & NativeMethods.ModAlt) != 0)
        {
            parts.Add("Alt");
        }

        if ((Modifiers & NativeMethods.ModShift) != 0)
        {
            parts.Add("Shift");
        }

        if ((Modifiers & NativeMethods.ModWin) != 0)
        {
            parts.Add("Win");
        }

        parts.Add(Key.ToString());
        return string.Join(" + ", parts);
    }
}
