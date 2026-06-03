using Content.Client._DEN.Consent.EntitySystems;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._DEN.Consent.Managers;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;

namespace Content.Client._DEN.Consent.UI;

[UsedImplicitly]
public sealed partial class ConsentUIController : UIController, IOnSystemChanged<ConsentSystem>, IOnStateChanged<GameplayState>, IOnStateChanged<LobbyState>
{
    [Dependency] private IConsentManager _consentManager = null!;
    [Dependency] private IInputManager _input = null!;

    public bool EverOpened = false;

    private ConsentWindow? _window;
    private ConsentSystem? _consentSystem;

    private MenuButton? GameConsentButton => UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.ConsentButton;
    private Button? LobbyConsentButton => (UIManager.ActiveScreen as LobbyGui)?.ConsentButton;

    public void OpenWindow()
    {
        if  (_window == null)
        {
            _window = new ConsentWindow();
            _window.OnOpen += OnOpen;
            _window.OnClose += OnClose;
        }

        if (EverOpened)
            _window.Open();
        else
            _window.OpenCentered();
    }

    public void CloseWindow()
    {
        if (_window is null)
            return;

        _window.OnOpen -= OnOpen;
        _window.OnClose -= OnClose;

        _window.Close();
    }

    public void ToggleWindow()
    {
        if (_window is null or { IsOpen: false })
        {
            OpenWindow();
            return;
        }

        CloseWindow();
    }

    private void SetConsentButtonPressed(bool pressed)
    {
        GameConsentButton?.Pressed = pressed;
        LobbyConsentButton?.Pressed = pressed;
    }

    private void ConsentButtonPressed(BaseButton.ButtonEventArgs args)
    {
        ToggleWindow();
    }

    private void OnOpen()
    {
        EverOpened = true;
        SetConsentButtonPressed(true);
    }

    private void OnClose()
    {
        SetConsentButtonPressed(false);
    }

    public void OnStateEntered(GameplayState state)
    {
        if (GameConsentButton is null)
            return;

        GameConsentButton.OnPressed += ConsentButtonPressed;
        GameConsentButton.Pressed = _window?.IsOpen ?? false;
    }

    public void OnStateExited(GameplayState state)
    {
        if (GameConsentButton is null)
            return;

        GameConsentButton.OnPressed -= ConsentButtonPressed;
    }

    public void OnStateEntered(LobbyState state)
    {
        if (LobbyConsentButton is null)
            return;

        LobbyConsentButton.OnPressed += ConsentButtonPressed;
        LobbyConsentButton.Pressed = _window?.IsOpen ?? false;
    }

    public void OnStateExited(LobbyState state)
    {
        if (LobbyConsentButton is null)
            return;

        LobbyConsentButton.OnPressed -= ConsentButtonPressed;
    }

    public void OnSystemLoaded(ConsentSystem system)
    {
        _consentSystem = system;
        _input.SetInputCommand(ContentKeyFunctions.OpenConsentWindow,
            InputCmdHandler.FromDelegate(_ => ToggleWindow()));
    }

    public void OnSystemUnloaded(ConsentSystem system)
    {
        _consentSystem = null;
        _input.SetInputCommand(ContentKeyFunctions.OpenConsentWindow, null);
    }
}
