using Content.Shared._DEN.Loadout;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class DenLoadoutTab
{
    public void OnClickEditLoadoutProfile(DenLoadout loadout)
    {
        OnTryCreateLoadout(loadout);
    }

    public void OnClickDeleteLoadoutProfile(DenLoadout loadout)
    {
        var profile = _profile?.WithoutLoadoutProfile(loadout);
        SetDirty(profile);
    }
}
