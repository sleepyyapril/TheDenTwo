using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Server.Discord.DiscordLink;
using Content.Server.GameTicking;
using Content.Shared._DEN.CCVar;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;

// ReSharper disable ArrangeTrailingCommaInMultilineLists

namespace Content.Server._DEN.Discord;

public sealed partial class DiscordAHelpRelay : IPostInjectInit
{
    [Dependency] private DiscordLink _discordLink = null!;
    [Dependency] private IEntityManager _entityManager = null!;
    [Dependency] private IConfigurationManager _configurationManager = null!;
    [Dependency] private ILocalizationManager _locManager = null!;
    [Dependency] private ILogManager _logManager = null!;

    public Action<NetUserId, Message>? ReplyReceived;

    private ISawmill _sawmill = null!;
    private const int MaxEmbedContentLength = 4000;
    private bool _enabled;
    private long _channelId = -1;

    private readonly Dictionary<NetUserId, ExistingAhelpRelay> _ahelpRelays = new();
    private readonly Dictionary<ulong, NetUserId> _threadsToUsers = new();

    public void Initialize()
    {
        var enabled = _configurationManager.GetCVar(DenCCVars.DiscordAhelpRelayEnabled);
        var channelId = _configurationManager.GetCVar(DenCCVars.DiscordAhelpChannelId);

        SetEnabled(enabled);
        SetChannelId(channelId);

        _discordLink.OnMessageReceived += OnMessageReceived;
    }

    public void Shutdown()
    {

    }

    private async void OnMessageReceived(Message message)
    {
        try
        {
            if (!_enabled
                || message.Author.IsBot
                || message.Channel is not GuildThread guildThread
                || guildThread.ParentId != (ulong) _channelId
                || !_threadsToUsers.TryGetValue(guildThread.Id, out var userId)
                || !_ahelpRelays.TryGetValue(userId, out var relay))
                return;

            ReplyReceived?.Invoke(relay.BwoinkUserId, message);
            await message.DeleteAsync();
        }
        catch (Exception e)
        {
            _sawmill.Error(e.ToStringBetter());
        }
    }

    public async void OnBwoink(ICommonSession session, BwoinkDiscordData ev)
    {
        try
        {
            if (!_enabled || _channelId == -1
                || !ulong.TryParse(_channelId.ToString(), out var channelId))
            {
                _sawmill.Error("AHelp relay not enabled or channel ID not specified correctly.");
                return;
            }
            var nextLine = GetNextLineFormatted(ev);
            var discordMessage = await GetDiscordMessage(session, ev, channelId, nextLine.Length);

            if (discordMessage == null)
            {
                _sawmill.Error(
                    $"No discord message found for specified channel ID: {channelId}. Does the channel exist?");
                return;
            }

            await SendNextMessage(session, discordMessage, ev, nextLine);
        }
        catch (Exception e)
        {
            _sawmill.Error(e.ToString());
        }
    }

    private string GetNextLineFormatted(BwoinkDiscordData ev)
    {
        var stringBuilder = new StringBuilder();

        if (ev.SenderIsAdmin)
            stringBuilder.Append(":outbox_tray:");
        else if (ev.NoReceivers)
            stringBuilder.Append(":sos:");
        else
            stringBuilder.Append(":inbox_tray:");

        if (ev.RoundTime != string.Empty && ev.RunLevel == GameRunLevel.InRound)
            stringBuilder.Append($" **{ev.RoundTime}**");

        if (!ev.PlaySound)
        {
            var adminOnlyText = _locManager.GetString("bwoink-message-admin-only");
            var silentText = _locManager.GetString("bwoink-message-silent");

            stringBuilder.Append(
                $" **{(ev.AdminOnly ? adminOnlyText : silentText)}**");
        }

        if (ev.DiscordName != string.Empty)
        {
            stringBuilder.Append($" **{ev.DiscordName}** ");
        }
        else
        {
            stringBuilder.Append($" **{ev.Username}** ");
        }

        stringBuilder.Append(ev.Text);

        return stringBuilder.ToString();
    }

    private async Task<RestMessage?> GetDiscordMessage(ICommonSession session,
        BwoinkDiscordData data,
        ulong channelId,
        int neededLength)
    {
        if (_ahelpRelays.TryGetValue(session.UserId, out var maybeExistingRelay)
            && maybeExistingRelay.LastRunLevel == data.RunLevel)
        {
            var restMessage = await _discordLink.GetMessageAsync(maybeExistingRelay.ChannelId, maybeExistingRelay.MessageId);

            // ReSharper disable once DuplicatedSequentialIfBodies
            if (restMessage != null
                && restMessage.Embeds.Count == 0)
                return restMessage;

            if (restMessage != null
                && restMessage.Embeds.Count > 0
                && restMessage.Embeds[0].Description == null)
                return restMessage;

            // ReSharper disable once DuplicatedSequentialIfBodies
            if (restMessage != null
                && restMessage.Embeds.Count > 0
                && restMessage.Embeds[0].Description != null
                && restMessage.Embeds[0].Description?.Length + neededLength < MaxEmbedContentLength)
                return restMessage;
        }

        var message = new MessageProperties
        {
            Content = $"AHelp from {data.Username}"
        };

        var messageAttempt = await _discordLink.SendMessageAsync(channelId, message);
        if (messageAttempt != null)
        {
            var thread = await messageAttempt
                .CreateGuildThreadAsync(new GuildThreadFromMessageProperties("Replies"));

            await thread.SendMessageAsync("To send an AHelp that plays the bwoink sound, prefix your message with ``$``." +
                                          "\nTo send an AHelp that can only be seen by admins, prefix your message with ``%``.");
            _threadsToUsers.Add(thread.Id, data.UserId);
        }

        return messageAttempt;
    }

    private async Task SendNextMessage(ICommonSession session,
        RestMessage msg,
        BwoinkDiscordData ev,
        string nextLine)
    {
        var embed = GenerateEmbed(session, msg, ev, nextLine);
        var newMessage = await msg.ModifyAsync(options =>
        {
            options.Embeds = [embed];
        });

        _ahelpRelays[session.UserId] = new ExistingAhelpRelay
        {
            BwoinkUserId = ev.UserId,
            ChannelId = (ulong) _channelId,
            MessageId = newMessage.Id,
            LastRunLevel = ev.RunLevel
        };
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public void SetChannelId(long channelId)
    {
        _channelId = channelId;
    }

    private EmbedProperties GenerateEmbed(ICommonSession session,
        RestMessage msg,
        BwoinkDiscordData bwoinkMessage,
        string nextMessage)
    {
        var priorText = msg.Embeds.FirstOrDefault()?.Description ?? string.Empty;
        var ticker = _entityManager.System<GameTicker>();
        var runLevel = _locManager.GetString($"game-run-level-{ticker.RunLevel}");
        var round = ticker.RoundId;
        var color = bwoinkMessage.EmbedColor;
        var embed = new EmbedProperties
        {
            Color = new NetCord.Color(color.RByte, color.GByte, color.BByte),
            Description = GenerateContent(priorText, nextMessage),
            Footer = new EmbedFooterProperties
            {
                Text = $"{runLevel} #{round}"
            },
            Timestamp = DateTimeOffset.FromUnixTimeSeconds((int) ticker.RoundStartTimeSpan.TotalSeconds)
        };

        return embed;
    }

    private string GenerateContent(string priorText,
        string nextLine)
    {
        var content = new StringBuilder();

        if (priorText != string.Empty)
            content.AppendLine(priorText);

        content.AppendLine(nextLine);
        return content.ToString();
    }

    public void PostInject()
    {
        _sawmill = _logManager.GetSawmill("discord-ahelp-relay");
    }
}

public sealed class ExistingAhelpRelay
{
    public NetUserId BwoinkUserId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
    public GameRunLevel LastRunLevel;
}
