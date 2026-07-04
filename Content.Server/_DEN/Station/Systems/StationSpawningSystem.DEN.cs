using Content.Shared._DEN.Loadout;
using Content.Shared.CCVar;
using Content.Shared.DetailExaminable;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Station.Systems;

public sealed partial class StationSpawningSystem
{
    private const string LoadoutTest1 = "test1";
    private const string LoadoutTest2 = "test2";

    private readonly HashSet<ProtoId<EntityLoadoutPrototype>> _testingLoadouts = new()
    {
        LoadoutTest1, LoadoutTest2
    };

    public EntityUid SpawnPlayerMob(
        EntityCoordinates coordinates,
        ProtoId<JobPrototype>? job,
        HumanoidCharacterProfile? profile,
        EntityUid? station,
        EntityUid? entity = null)
    {
        string speciesId = profile?.Species ?? HumanoidCharacterProfile.DefaultSpecies;

        if (!_prototypeManager.TryIndex<SpeciesPrototype>(speciesId, out var species))
            throw new ArgumentException($"Invalid species prototype was used: {speciesId}");

        entity ??= Spawn(species.Prototype, coordinates);

        if (profile != null)
        {
            _visualBody.ApplyProfileTo(entity.Value, profile);
            _humanoidProfile.ApplyProfileTo(entity.Value, profile);
            _metaSystem.SetEntityName(entity.Value, profile.Name);

            if (profile.FlavorText != "" && _configurationManager.GetCVar(CCVars.FlavorText))
            {
                AddComp<DetailExaminableComponent>(entity.Value).Content = profile.FlavorText;
            }

            SpawnCharacterLoadout(_testingLoadouts, entity.Value, coordinates);
        }

        return entity.Value;
    }
}
