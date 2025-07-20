using NadekoBot.Medusa;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace VoiceReplaceMedusa;

public sealed class VoiceReplace : Snek
{
    private readonly string _basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    private HttpClient _httpClient;
    private MedusaConfig _config;
    private IDeserializer _deserializer;
    private ISerializer _serializer;
    private readonly string _configYml = "config.yml";
    public async override ValueTask InitializeAsync()
    {
        try
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(300);
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            _config = _deserializer.Deserialize<MedusaConfig>(File.ReadAllText(Path.Combine(_basePath, _configYml)));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, ex.StackTrace);
        }
        return;
    }
    public async override ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        return;
    }

    [cmd]
    public async Task Voices(AnyContext ctx)
    {
        var url = new Uri($"{_config.Api}/voices");
        using HttpResponseMessage response = await _httpClient.GetAsync(url);
        var voices = await response.Content.ReadAsStringAsync();
        var voiceArray = JsonConvert.DeserializeObject<string[]>(voices);
        await ctx.SendConfirmAsync($"Here's the list of voices: {string.Join(", ", voiceArray)}");
    }

    [cmd]
    public async Task VoiceReplacerYt(AnyContext ctx, string url, string voice, string pitch = "0")
    {
        try
        {
            var testurl = new Uri(url);
        }
        catch
        {
            await ctx.SendErrorAsync("Url is not valid.");
            return;
        }
        var voiceinforesponse = await _httpClient.GetAsync($"{_config.Api}/voiceinfo/{voice}");
        if (!voiceinforesponse.IsSuccessStatusCode)
        {
            await ctx.SendErrorAsync("Voice not found");
        }
        var voiceinfo = JsonConvert.DeserializeObject<VoiceInfo>(await voiceinforesponse.Content.ReadAsStringAsync());
        var voiceavatarurl = new Uri($"{_config.Api}{voiceinfo.AvatarUrl}");
        var avatarResponse = await _httpClient.GetAsync(voiceavatarurl);
        Stream avatar;
        if (!avatarResponse.IsSuccessStatusCode)
        {
            avatar = null;
        }
        avatar = await avatarResponse.Content.ReadAsStreamAsync();
        var httpResponse = await _httpClient.PostAsJsonAsync($"{_config.Api}/ytinfo", new InfoRequest(url));
        if (!httpResponse.IsSuccessStatusCode)
        {
            await ctx.SendErrorAsync("Failed to replace vocals.");
            return;
        }
        var infoContent = await httpResponse.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<VideoInfo>(infoContent);
        string title = info.Title;
        var msgToGen = await ctx.Channel.SendMessageAsync($"Requesting {voiceinfo.DisplayName} to cover {title}.");
        httpResponse = await _httpClient.PostAsJsonAsync($"{_config.Api}/replace_yt", new ReplaceYtRequest(url, voice, pitch));
        var webhookSender = new WebhookSender(ctx.Channel, voiceinfo.DisplayName, avatar);
        if (httpResponse.IsSuccessStatusCode)
        {
            try
            {
                var responseContent = await httpResponse.Content.ReadAsStreamAsync();
                var ext = MimeTypes.GetMimeTypeExtensions(httpResponse.Content.Headers.ContentType.ToString()).First();
                var filename = CleanFileName($"{voiceinfo.DisplayName} {title}.{ext}");
                await webhookSender.SendFileAsync(responseContent, filename);
                await webhookSender.DisposeAsync();
                await msgToGen.DeleteAsync();
            }
            catch
            {
                await msgToGen.DeleteAsync();
                await ctx.SendErrorAsync($"Failed to upload result. File is most likely to large.");
                return;
            }
        }
        else
        {
            await msgToGen.DeleteAsync();
            await ctx.SendErrorAsync("Failed to replace vocals.");
        }
    }

    [cmd]
    [bot_owner_only]
    public async Task VoiceReplacerApi(AnyContext ctx, string url = "")
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            await ctx.SendConfirmAsync($"api: {_config.Api}");
            return;
        }
        _config.Api = url.EndsWith('/') ? url.Substring(0, url.Length - 1) : url;
        File.WriteAllText(Path.Combine(_basePath, _configYml), _serializer.Serialize(_config));
        await ctx.ConfirmAsync();
    }

    public static string CleanFileName(string fileName)
    {
        return Regex.Replace(fileName, "<|>|:|\"|\\/|\\|||\\?\\*", "");
    }

}

public class MedusaConfig
{
    public string Api { get; set; }
}