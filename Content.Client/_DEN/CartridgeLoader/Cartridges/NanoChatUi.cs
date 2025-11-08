using Content.Client.UserInterface.Fragments;
using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatUi : UIFragment
{
    private NanoChatUiFragment? _fragment;
    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not NanoChatUiState castState)
            return;

        _fragment?.UpdateState(castState);
    }
}
