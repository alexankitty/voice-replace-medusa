namespace VoiceReplaceMedusa;
public class MimeTypes
{
    public static readonly Dictionary<string, string> s_typeMap;
    static MimeTypes()
    {
        s_typeMap = new Dictionary<string, string>(1180, StringComparer.OrdinalIgnoreCase)
        {
            { "3gpp", "audio/3gpp" },
            { "aac", "audio/aac" },
            { "adp", "audio/adpcm" },
            { "adts", "audio/aac" },
            { "aep", "application/vnd.audiograph" },
            { "aif", "audio/x-aiff" },
            { "aifc", "audio/x-aiff" },
            { "aiff", "audio/x-aiff" },
            { "amr", "audio/amr" },
            { "au", "audio/basic" },
            { "caf", "audio/x-caf" },
            { "dra", "audio/vnd.dra" },
            { "dts", "audio/vnd.dts" },
            { "dtshd", "audio/vnd.dts.hd" },
            { "eol", "audio/vnd.digital-winds" },
            { "flac", "audio/x-flac" },
            { "kar", "audio/midi" },
            { "lvp", "audio/vnd.lucent.voice" },
            { "m2a", "audio/mpeg" },
            { "m3a", "audio/mpeg" },
            { "m3u", "audio/x-mpegurl" },
            { "m4a", "audio/mp4" },
            { "mid", "audio/midi" },
            { "midi", "audio/midi" },
            { "mka", "audio/x-matroska" },
            { "mp2", "audio/mpeg" },
            { "mp2a", "audio/mpeg" },
            { "mp3", "audio/mp3" },
            { "mp4a", "audio/mp4" },
            { "mpga", "audio/mpeg" },
            { "mxmf", "audio/mobile-xmf" },
            { "oga", "audio/ogg" },
            { "ogg", "audio/ogg" },
            { "opus", "audio/ogg" },
            { "pya", "audio/vnd.ms-playready.media.pya" },
            { "ra", "audio/x-realaudio" },
            { "ram", "audio/x-pn-realaudio" },
            { "rip", "audio/vnd.rip" },
            { "rmi", "audio/midi" },
            { "rmp", "audio/x-pn-realaudio-plugin" },
            { "s3m", "audio/s3m" },
            { "saf", "application/vnd.yamaha.smaf-audio" },
            { "sil", "audio/silk" },
            { "snd", "audio/basic" },
            { "spx", "audio/ogg" },
            { "uva", "audio/vnd.dece.audio" },
            { "uvva", "audio/vnd.dece.audio" },
            { "wav", "audio/wav" },
            { "wax", "audio/x-ms-wax" },
            { "weba", "audio/webm" },
            { "wma", "audio/x-ms-wma" },
            { "xm", "audio/xm" },
            { "zmm", "application/vnd.handheld-entertainment+xml" }
        };
    }
    public static IEnumerable<string> GetMimeTypeExtensions(string mimeType)
    {
        if (mimeType is null)
        {
            throw new ArgumentNullException(nameof(mimeType));
        }

        return s_typeMap
            .Where(keyPair => string.Equals(keyPair.Value, mimeType, StringComparison.OrdinalIgnoreCase))
            .Select(keyPair => keyPair.Key);
    }
}