using Content.Client.UserInterface.Controls;
using Robust.Shared.Player;

namespace Content.Client._DEN.Dialog;

public sealed class DialogOptionPressedArgs
{
    public required ICommonSession? Session { get; set; }
    public required FancyWindow DialogWindow {  get; set; }
}

public abstract class DialogOptionBase(LocId text, Action<DialogOptionPressedArgs> onPressed)
{
    public LocId Text = text;
    public Action<DialogOptionPressedArgs> OnPressed = onPressed;
}

public sealed class DialogOption(LocId text, Action<DialogOptionPressedArgs> onPressed) : DialogOptionBase(text, onPressed);

public sealed class CancelDialogOption : DialogOptionBase
{
    private static readonly LocId CancelText = "generic-dialog-cancel";

    public CancelDialogOption() : base(CancelText, OnCancelPressed)
    {
    }

    public CancelDialogOption(LocId text) : base(text, OnCancelPressed)
    {
    }

    private static void OnCancelPressed(DialogOptionPressedArgs args)
    {
        args.DialogWindow.Close();
    }
}
