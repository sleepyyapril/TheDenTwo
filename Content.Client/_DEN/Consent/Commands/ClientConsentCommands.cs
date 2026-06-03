using Content.Client._DEN.Consent.UI;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;

namespace Content.Client._DEN.Consent.Commands;

[AnyCommand]
public sealed partial class ConsentPrefsCommand : LocalizedCommands
{
    [Dependency] private IUserInterfaceManager _ui = null!;

    public override string Command => "consentprefs";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player == null)
            return;

        var consentUi = _ui.GetUIController<ConsentUIController>();
        consentUi.ToggleWindow();
    }
}
