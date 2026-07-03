using Robust.Shared.Prototypes;

namespace Content.Server._DEN.Loadout.Modules;

public interface ILoadoutModule
{
    void OnEntitySpawn(IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityUid uid);
}

