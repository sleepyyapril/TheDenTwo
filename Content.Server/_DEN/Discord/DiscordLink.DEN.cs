using System.Threading.Tasks;
using NetCord;
using NetCord.Rest;

namespace Content.Server.Discord.DiscordLink;

public sealed partial class DiscordLink
{
    public async Task<RestMessage?> GetMessageAsync(ulong channelId, ulong messageId)
    {
        if (_client == null)
            return null;

        var message = await _client.Rest.GetMessageAsync(channelId, messageId);
        return message.Author.Id != _client.Id ? null : message;
    }

    public async Task<RestMessage?> SendMessageAsync(ulong channelId, MessageProperties message)
    {
        if (_client == null)
        {
            return null;
        }

        var channel = await _client.Rest.GetChannelAsync(channelId);
        if (channel is TextChannel channelAsT)
            return await channelAsT.SendMessageAsync(message);

        _sawmill.Error("Tried to send a message to Discord but the channel {Channel} was not found.", channel);
        return null;
    }

    public async Task<RestMessage?> EditMessageAsync(ulong channelId, ulong messageId, MessageProperties newMessage)
    {
        if (_client == null)
            return null;

        var message = await _client.Rest.GetMessageAsync(channelId, messageId);

        if (message.Author.Id != _client.Id)
            return null;

        var editedMessage = await message.ModifyAsync(options =>
        {
            options.Content = newMessage.Content;
            options.Embeds = newMessage.Embeds;
            options.Components = newMessage.Components;
        });

        return editedMessage;
    }
}
