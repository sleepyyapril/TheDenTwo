using Robust.Shared.Player;

namespace Content.Client._DEN.Dialog;

public class DialogOptionPressedArgs
{
    public required ICommonSession Session { get; set; }
}

public abstract class DialogOption
{
    public LocId Text { get; set; } = string.Empty;
    public required Action< OnPressed { get; set; }
}

public class CancelDialogOption
{
    public LocId Text { get; set; } = string.Empty;
    public Action OnPressed { get; set; }
}
