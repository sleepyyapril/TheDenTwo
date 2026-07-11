using Content.Shared._DEN.Loadout;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class LoadoutProfilesContainer
{
    private void OnClickEditLoadoutProfile(DenLoadout loadout)
    {
        OnTryCreateLoadout(loadout);
    }

    private void OnClickDeleteLoadoutProfile(DenLoadout loadout)
    {
        _profile = _profile?.WithoutLoadoutProfile(loadout);
        SetDirty(_profile);

        RefreshCategories();
    }
}
