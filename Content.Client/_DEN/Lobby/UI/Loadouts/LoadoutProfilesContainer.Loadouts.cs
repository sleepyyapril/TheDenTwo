using Content.Shared._DEN.Loadout;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class LoadoutProfilesContainer
{
    private void OnClickEditLoadouts(DenLoadout loadout)
    {
        LoadoutItemSelection.Visible = !LoadoutContainer.Visible;
        LoadoutProfileSelection.Visible = !LoadoutContainer.Visible;
    }
}
