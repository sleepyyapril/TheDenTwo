using Content.Server._DEN.Discord;

namespace Content.Server._DEN.Entry;

public sealed partial class DenEntryPoint
{
    [Dependency] private DiscordAHelpRelay _discordAhelpRelay = null!;
    [Dependency] private DiscordCommands _discordCommands = null!;

    public void Init()
    {
        _discordAhelpRelay.Initialize();
        _discordCommands.Initialize();
    }

    public void Disposing()
    {
        _discordAhelpRelay.Shutdown();
    }
}
