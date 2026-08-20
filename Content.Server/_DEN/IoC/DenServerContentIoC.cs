using Content.Server._DEN.Denu;
using Content.Server._DEN.Discord;
using Content.Server._DEN.Entry;
using Content.Server._DEN.Requirements.Managers;
using Content.Shared._DEN.Requirements.Managers;

// ReSharper disable once CheckNamespace
namespace Content.Server.IoC;

internal sealed class DenServerContentIoC
{
    public static void Register(IDependencyCollection deps)
    {
        deps.Register<IDenuSettingsManager, DenuSettingsManager>(); // DEN
        deps.Register<IPlayerRequirementManager, PlayerRequirementManager>(); // DEN
        deps.Register<DiscordCommands>(); // DEN
        deps.Register<DenEntryPoint>(); // DEN
        deps.Register<DiscordAHelpRelay>(); // DEN
    }
}
