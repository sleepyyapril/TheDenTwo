using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Loadout;

[Serializable, NetSerializable]
public record struct DenLoadout
{
    public Guid Id;
    public int Priority;

    public Dictionary<Guid, ProtoId<LoadoutPrototype>> Loadouts;
    public Dictionary<Guid, DenCustomLoadoutInfo> CustomLoadouts;
}

[Serializable, NetSerializable]
public record struct DenCustomLoadoutInfo
{
    public string? CustomName;
    public string? CustomDescription;
    public string? CustomColor;
}
