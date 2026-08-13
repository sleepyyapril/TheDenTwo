namespace Content.Client._DEN.Dialog;

public interface IDialogManager
{
    void SendDialog(LocId title, LocId text, List<DialogOptionBase> options);
}

public sealed class DialogManager : IDialogManager
{
    public void SendDialog(LocId title, LocId text, List<DialogOptionBase> options)
    {
        var dialogWindow = new DialogWindow()
        {
            Title = Loc.GetString(title)
        };

        dialogWindow.SetText(text);

        foreach (var option in options)
        {
            dialogWindow.AddButton(option.Text, option.OnPressed);
        }

        dialogWindow.OpenCentered();
    }
}

