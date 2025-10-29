using Content.Shared._DEN.Consent.Managers;
using Content.Shared._DEN.Consent.Prototypes;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._DEN.Consent;

[AnyCommand]
public sealed class SetConsentCommand : LocalizedCommands
{
    [Dependency] private readonly IConsentManager _consentManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    public override string Command { get; } = "setconsent";
    public override string Description { get; } = "Sets a consent ID to a value for yourself.";
    public override string Help { get; } = "setconsent consentId newValue";
    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2 || shell.Player == null)
            return;

        if (!_protoManager.TryIndex<ConsentTogglePrototype>(args[0], out var toggle))
            return;

        if (!bool.TryParse(args[1], out var newValue))
            return;

        _consentManager.SetConsentToggle(shell.Player.UserId, args[0], newValue);
        shell.WriteLine($"Set consent `{args[0]}` to `{args[1]}`");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = CompletionHelper.PrototypeIDs<ConsentTogglePrototype>();
            return CompletionResult.FromOptions(options);
        }

        if (args.Length == 2)
        {
            var options = CompletionHelper.Booleans;
            return CompletionResult.FromOptions(options);
        }

        return CompletionResult.Empty;
    }
}

[AnyCommand]
public sealed class ConsentCommand : LocalizedCommands
{
    [Dependency] private readonly IConsentManager _consentManager = default!;

    public override string Command { get; } = "consents";
    public override string Description { get; } = "Gets consent value.";
    public override string Help { get; } = "consents";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1 || shell.Player == null)
            return;

        var consents = _consentManager.GetConsentToggles(shell.Player.UserId);
        var consentsMessage = string.Join("\n -", consents);

        if (consents.Count == 0)
        {
            shell.WriteLine("No different consents; all are using default.");
            return;
        }

        shell.WriteLine($"Different consents: \n- {consentsMessage}");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = CompletionHelper.PrototypeIDs<ConsentTogglePrototype>();
            return CompletionResult.FromOptions(options);
        }

        return CompletionResult.Empty;
    }
}
