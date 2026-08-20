using Robust.Shared.Configuration;

namespace Content.Shared._DEN.CCVar;

public sealed partial class DenCCVars
{
    /// <summary>
    ///     Game-to-discord AHelp relay. If enabled, disables webhook relay.
    /// </summary>
    public static readonly CVarDef<bool> DiscordAhelpRelayEnabled =
        CVarDef.Create("den.discord.ahelp_relay_enabled", true, CVar.SERVERONLY);

    /// <summary>
    ///     Game-to-discord AHelp relay. If enabled, disables webhook relay.
    /// </summary>
    public static readonly CVarDef<long> DiscordAhelpChannelId =
        CVarDef.Create("den.discord.ahelp_relay_channel_id", (long) -1, CVar.SERVERONLY);
}
