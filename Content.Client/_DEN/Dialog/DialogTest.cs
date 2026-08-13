using Robust.Shared.Console;

namespace Content.Client._DEN.Dialog;

public sealed partial class DialogTest : IConsoleCommand
{
    [Dependency] private IDialogManager _dialogManager = null!;

    public string Command => "dialogtest";
    public string Description => "Opens a dialog window.";
    public string Help => "dialogtest";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var dialogOptionYes = new DialogOption("dialog-test-yes", pressedArgs => Disconnect(shell, pressedArgs));
        var dialogOptionNo = new CancelDialogOption("dialog-test-no");
        var dialogOptions = new List<DialogOptionBase> { dialogOptionYes, dialogOptionNo };

        _dialogManager.SendDialog("dialog-test-title", "dialog-test-text", dialogOptions);
    }

    private void Disconnect(IConsoleShell shell, DialogOptionPressedArgs args)
    {
        args.DialogWindow.Close();
        shell.ExecuteCommand("disconnect");
    }
}
