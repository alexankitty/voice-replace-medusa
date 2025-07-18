using System.Text;
using NadekoBot.Medusa;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Net.Http.Json;
using Discord;

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
        var typing = ctx.Channel.EnterTypingState();
        var httpResponse = await _httpClient.PostAsJsonAsync($"{_config.Api}/ytinfo", new InfoRequest(url));
        if (!httpResponse.IsSuccessStatusCode)
        {
            typing.Dispose();
            return;
        }
        var infoContent = await httpResponse.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<VideoInfo>(infoContent);
        string title = info.Title;
        httpResponse = await _httpClient.PostAsJsonAsync($"{_config.Api}/replace_yt", new ReplaceYtRequest(url, voice, pitch));
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseContent = await httpResponse.Content.ReadAsStreamAsync();
            var ext = MimeTypes.GetMimeTypeExtensions(httpResponse.Content.Headers.ContentType.ToString()).First();
            await ctx.Channel.SendFileAsync(responseContent, $"{char.ToUpper(voice[0]) + voice.Substring(1)} {title}.{ext}");
        }
        else
        {
            await ctx.SendErrorAsync("Failed to replace vocals.");
        }
        typing.Dispose();
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
}

public class MedusaConfig
{
    public string Api { get; set; }
}
