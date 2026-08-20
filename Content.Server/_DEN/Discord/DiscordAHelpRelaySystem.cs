using System.Linq;
using Content.Server.Administration.Managers;
using Content.Server.Administration.Systems;
using Content.Server.Afk;
using Content.Server.GameTicking;
using Content.Shared._DEN.CCVar;
using Content.Shared.Administration;
using NetCord.Gateway;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Server._DEN.Discord;

/// <summary>
/// This handles entity subscriptions for the Discord AHelp relay.
/// </summary>
public sealed partial class DiscordAHelpRelaySystem : EntitySystem
{
    [Dependency] private DiscordAHelpRelay _relay = null!;
    [Dependency] private GameTicker _ticker = null!;
    [Dependency] private IAdminManager _adminManager = null!;
    [Dependency] private IAfkManager _afkManager = null!;
    [Dependency] private IConfigurationManager _configurationManager = null!;
    [Dependency] private IPlayerManager _playerManager = null!;

    private readonly Color _activeAdminsColor = Color.LimeGreen;
    private readonly Color _noAdminsColor = Color.Red;

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_configurationManager, DenCCVars.DiscordAhelpRelayEnabled, _relay.SetEnabled, true);
        Subs.CVar(_configurationManager, DenCCVars.DiscordAhelpChannelId, _relay.SetChannelId, true);

        _relay.ReplyReceived += ReplyReceived;
    }

    private void ReplyReceived(NetUserId toBwoink, Message msg)
    {
        if (!_playerManager.TryGetSessionById(toBwoink, out var player))
            return;

        var admins = GetTargetAdmins();
        var playSound = false;
        var adminOnly = false;
        var trueContent = msg.Content;

        if (msg.Content.StartsWith('$') && msg.Content.Length > 1)
        {
            playSound = true;
            adminOnly = false;
            trueContent = msg.Content[1..];
        }

        if (msg.Content.StartsWith('%') && msg.Content.Length > 1)
        {
            adminOnly = true;
            playSound = false;
            trueContent = msg.Content[1..];
        }

        var ev = new SharedBwoinkSystem.BwoinkTextMessage(toBwoink,
            SharedBwoinkSystem.SystemUserId,
            $"[color=red][bold](D)[/bold] {msg.Author.Username}[/color]: {trueContent}",
            DateTime.Now,
            playSound,
            adminOnly);

        var receiverIsAdmin = false;
        foreach (var admin in admins)
        {
            if (admin == player.Channel)
            {
                receiverIsAdmin = true;
                continue;
            }

            RaiseNetworkEvent(ev, admin);
        }

        if (!adminOnly || receiverIsAdmin)
            RaiseNetworkEvent(ev, player.Channel);

        var name = string.Empty;
        var nonAfkAdmins = GetNonAfkAdmins();
        var color = nonAfkAdmins.Count > 0 ? _activeAdminsColor : _noAdminsColor;
        var roundTime = _ticker.RoundDuration().ToString("hh\\:mm\\:ss");
        var roundState = _ticker.RunLevel;

        var data = new BwoinkDiscordData(ev.UserId,
            ev.TrueSender,
            name,
            trueContent,
            roundTime,
            roundState,
            ev.SentAt,
            ev.PlaySound,
            ev.AdminOnly,
            nonAfkAdmins.Count == 0,
            true,
            color,
            $"**(D) {msg.Author.Username}**");

        _relay.OnBwoink(player, data);
    }

    [SubscribeNetworkEvent]
    private void OnBwoink(SharedBwoinkSystem.BwoinkTextMessage ev, EntitySessionEventArgs args)
    {
        if (ev.UserId == SharedBwoinkSystem.SystemUserId)
            return;

        var name = args.SenderSession.Data.UserName;
        if (args.SenderSession.AttachedEntity is { Valid: true } ent
            && TryName(ent, out var entityName))
            name = $"{entityName} ({name})";

        var senderIsAdmin = _adminManager.HasAdminFlag(args.SenderSession, AdminFlags.Adminhelp);
        var nonAfkAdmins = GetNonAfkAdmins();
        var color = nonAfkAdmins.Count > 0 ? _activeAdminsColor : _noAdminsColor;
        var roundTime = _ticker.RoundDuration().ToString("hh\\:mm\\:ss");
        var roundState = _ticker.RunLevel;

        var data = new BwoinkDiscordData(ev.UserId,
            ev.TrueSender,
            name,
            ev.Text,
            roundTime,
            roundState,
            ev.SentAt,
            ev.PlaySound,
            ev.AdminOnly,
            nonAfkAdmins.Count == 0,
            senderIsAdmin,
            color,
            string.Empty);
        _relay.OnBwoink(args.SenderSession, data);
    }

    private IList<INetChannel> GetNonAfkAdmins()
    {
        return _adminManager.ActiveAdmins
            .Where(p => (_adminManager.GetAdminData(p)?.HasFlag(AdminFlags.Adminhelp) ?? false) &&
                        !_afkManager.IsAfk(p))
            .Select(p => p.Channel)
            .ToList();
    }

    private IList<INetChannel> GetTargetAdmins()
    {
        return _adminManager.ActiveAdmins
            .Where(p => _adminManager.GetAdminData(p)?.HasFlag(AdminFlags.Adminhelp) ?? false)
            .Select(p => p.Channel)
            .ToList();
    }
}

/// <summary>
///     Information we got from BwoinkTextMessage and extras that only an in-sim system should be getting
/// </summary>
public sealed class BwoinkDiscordData(
    NetUserId userId,
    NetUserId trueSender,
    string username,
    string text,
    string roundTime,
    GameRunLevel runLevel,
    DateTime? sentAt,
    bool playSound,
    bool adminOnly,
    bool noReceivers,
    bool senderIsAdmin,
    Color embedColor,
    string discordName)
{
    public DateTime SentAt { get; } = sentAt ?? DateTime.Now;

    public NetUserId UserId { get; } = userId;

    // This is ignored from the client.
    // It's checked by the client when receiving a message from the server for bwoink noises.
    // This could be a boolean "Incoming", but that would require making a second instance.
    public NetUserId TrueSender { get; } = trueSender;

    public string Username { get; } = username;

    public string Text { get; } = text;

    public string RoundTime { get; } = roundTime;

    public GameRunLevel RunLevel { get; } = runLevel;

    public bool PlaySound { get; } = playSound;

    public readonly bool AdminOnly = adminOnly;

    public readonly bool NoReceivers = noReceivers;

    public readonly bool SenderIsAdmin = senderIsAdmin;

    public readonly Color EmbedColor = embedColor;

    public string DiscordName = discordName;
}
