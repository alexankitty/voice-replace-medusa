using Discord;
using Discord.Webhook;

namespace VoiceReplaceMedusa;

public class WebhookSender
{
    private DiscordWebhookClient _client;
    private IWebhook _webhook;
    private IIntegrationChannel _webhookChannel;
    private IMessageChannel _channel;
    public WebhookSender(IMessageChannel channel, string displayname, Stream avatar)
    {
        _channel = channel;
        _webhookChannel = channel as IIntegrationChannel;
        if (_webhookChannel is null) throw new NullReferenceException(nameof(_webhookChannel));
        _webhook = _webhookChannel.CreateWebhookAsync(displayname, avatar).Result;
        _client = new DiscordWebhookClient(_webhook.Id, _webhook.Token);
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _webhook?.DeleteAsync();
    }

    public async Task<IMessage> SendMessageAsync(string text)
    {
        ulong msgId = await _client.SendMessageAsync(text);
        return await _channel.GetMessageAsync(msgId);
    }
    public async Task<IMessage> SendFileAsync(Stream file, string fileName, string text = "")
    {
        ulong msgId = await _client.SendFileAsync(file, fileName, text: text);
        return await _channel.GetMessageAsync(msgId);
    }
}