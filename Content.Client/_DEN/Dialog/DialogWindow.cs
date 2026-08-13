using Content.Client.UserInterface.Controls;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._DEN.Dialog;

public sealed partial class DialogWindow : FancyWindow
{
    [Dependency] private IPlayerManager _playerManager = null!;

    private readonly BoxContainer _buttonContainer;
    private readonly RichTextLabel _textLabel;

    public DialogWindow()
    {
        Title = Loc.GetString("dialog-generic-confirm");

        var container = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical
        };

        ContentsContainer.AddChild(container);

        _textLabel = new RichTextLabel
        {
            HorizontalExpand = true,
            HorizontalAlignment = HAlignment.Center,
            Text = Loc.GetString("dialog-generic-missing-text")
        };

        container.AddChild(_textLabel);

        var separator = new Control
        {
            VerticalExpand = true
        };

        container.AddChild(separator);

        var buttonContainer = new BoxContainer
        {
            Margin = new Thickness(0, 7),
            Align = BoxContainer.AlignMode.Center
        };

        container.AddChild(buttonContainer);
        _buttonContainer = buttonContainer;

        SetWidth = 100;
        MaxWidth = 200;

        SetHeight = 50;
        MaxHeight = 250;

        InvalidateMeasure();
    }

    public void SetText(LocId locId)
    {
        _textLabel.Text = Loc.GetString(locId);
    }

    public void AddButton(LocId text, Action<DialogOptionPressedArgs> callback, bool confirmButton = false)
    {
        var button = confirmButton ? new ConfirmButton() : new Button();
        button.Text = Loc.GetString(text);
        button.InvalidateMeasure();

        var args = new DialogOptionPressedArgs
        {
            Session = _playerManager.LocalSession,
            // ReSharper disable once ArrangeTrailingCommaInMultilineLists
            DialogWindow = this
        };

        button.OnPressed += _ => callback.Invoke(args);
        _buttonContainer.AddChild(button);

        InvalidateMeasure();
    }
}
