using Content.Shared._DEN.Loadout;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class LoadoutProfilesContainer
{
    private void OnClickEditLoadoutProfile(DenLoadoutProfile loadoutProfile)
    {
        OnTryCreateLoadout(loadoutProfile);
    }

    private void OnClickDeleteLoadoutProfile(DenLoadoutProfile loadoutProfile)
    {
        _profile = _profile?.WithoutLoadoutProfile(loadoutProfile);
        SetDirty(_profile);

        RefreshCategories();
    }
}
