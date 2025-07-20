using System.Text.Json.Serialization;
namespace VoiceReplaceMedusa;

public class ReplaceYtRequest
{
    public ReplaceYtRequest(string url, string voice, string pitch)
    {
        Url = url;
        Voice = voice;
        Pitch = pitch;
    }

    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("voice")]
    public string Voice { get; set; }
    [JsonPropertyName("pitch")]
    public string Pitch { get; set; }
}

public class VideoInfo
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

public class InfoRequest
{
    public InfoRequest(string url)
    {
        Url = url;
    }
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class VoiceInfo
{
    [JsonPropertyName("displayname")]
    public string DisplayName { get; set; }
    [JsonPropertyName("avatarurl")]
    public string AvatarUrl { get; set; }
}