using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Loadout;

[Serializable, NetSerializable]
public record struct DenLoadout
{
    public Guid Id;
    public int Priority;
    public string Name;
    public Guid LoadoutCategory;

    public HashSet<ProtoId<EntityLoadoutPrototype>> Loadouts;
    //public Dictionary<Guid, HashSet<DenCustomLoadoutInfo>> CustomLoadouts;
}

[Serializable, NetSerializable]
public record struct DenCustomLoadoutInfo
{
    public EntProtoId EntProtoId;

    public string? CustomName;
    public string? CustomDescription;
    public string? CustomColor;
}

[Serializable, NetSerializable]
public record struct DenLoadoutCategory
{
    public Guid Id;
    public int Priority;
    public string Name;
    public string Color;
    public HashSet<Guid> Members;
}
