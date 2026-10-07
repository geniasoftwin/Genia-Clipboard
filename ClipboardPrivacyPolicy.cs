namespace GeniaClipboard;

internal static class ClipboardPrivacyPolicy
{
    private const string ExcludeFromMonitorFormat = "ExcludeClipboardContentFromMonitorProcessing";
    private const string CanIncludeInHistoryFormat = "CanIncludeInClipboardHistory";

    public static bool ShouldIgnore(IDataObject dataObject, bool respectWindowsPrivacyMarkers)
    {
        if (!respectWindowsPrivacyMarkers)
        {
            return false;
        }

        try
        {
            if (dataObject.GetDataPresent(ExcludeFromMonitorFormat, autoConvert: false))
            {
                return true;
            }

            if (!dataObject.GetDataPresent(CanIncludeInHistoryFormat, autoConvert: false))
            {
                return false;
            }

            var value = dataObject.GetData(CanIncludeInHistoryFormat, autoConvert: false);
            return !ReadBooleanDword(value);
        }
        catch
        {
            // A privacy marker that cannot be interpreted is treated conservatively.
            return true;
        }
    }

    private static bool ReadBooleanDword(object? value)
    {
        return value switch
        {
            bool boolean => boolean,
            byte number => number != 0,
            short number => number != 0,
            ushort number => number != 0,
            int number => number != 0,
            uint number => number != 0,
            long number => number != 0,
            ulong number => number != 0,
            byte[] bytes when bytes.Length >= 4 => BitConverter.ToInt32(bytes, 0) != 0,
            Stream stream => ReadStreamDword(stream),
            _ => false
        };
    }

    private static bool ReadStreamDword(Stream stream)
    {
        var originalPosition = stream.CanSeek ? stream.Position : 0;
        Span<byte> buffer = stackalloc byte[4];

        try
        {
            var read = stream.Read(buffer);
            return read == 4 && BitConverter.ToInt32(buffer) != 0;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }
}
