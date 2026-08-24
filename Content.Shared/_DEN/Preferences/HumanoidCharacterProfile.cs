using Content.Shared._DEN.Loadout;
using Content.Shared._DEN.Traits.Prototypes;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Traits;
using JetBrains.Annotations;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences;

public sealed partial class HumanoidCharacterProfile
{
    [DataField("_traitPreferences")]
    private HashSet<ProtoId<EntityTraitPrototype>> _entityTraitPreferences = new();

    /// <summary>
    /// <see cref="_entityTraitPreferences"/>
    /// </summary>
    public IReadOnlySet<ProtoId<EntityTraitPrototype>> EntityTraitPreferences => _entityTraitPreferences;

    [DataField("_loadoutCategories")]
    private Dictionary<Guid, LoadoutProfileCategory> _loadoutCategories = new();

    public IReadOnlyDictionary<Guid, LoadoutProfileCategory> LoadoutCategories => _loadoutCategories;

    [DataField("_loadoutProfiles")]
    private Dictionary<Guid, DenLoadoutProfile> _loadoutProfiles = new();

    public IReadOnlyDictionary<Guid, DenLoadoutProfile> LoadoutProfiles => _loadoutProfiles;

    [DataField("_jobLoadouts")]
    private Dictionary<ProtoId<JobPrototype>, HashSet<Guid>> _jobLoadouts = new();

    public IReadOnlyDictionary<ProtoId<JobPrototype>, HashSet<Guid>> JobLoadouts => _jobLoadouts;

    public HumanoidCharacterProfile(
        string name,
        string flavortext,
        string species,
        int age,
        Sex sex,
        ProtoId<EmoteSoundsPrototype> voice,
        Gender gender,
        HumanoidCharacterAppearance appearance,
        SpawnPriorityPreference spawnPriority,
        Dictionary<ProtoId<JobPrototype>, JobPriority> jobPriorities,
        PreferenceUnavailableMode preferenceUnavailable,
        HashSet<ProtoId<AntagPrototype>> antagPreferences,
        HashSet<ProtoId<EntityTraitPrototype>> entityTraitPreferences,
        // Dictionary<string, RoleLoadout> loadouts, - DEN
        Dictionary<Guid, LoadoutProfileCategory> loadoutCategories,
        Dictionary<Guid, DenLoadoutProfile> loadoutProfiles,
        Dictionary<ProtoId<JobPrototype>, HashSet<Guid>> jobLoadouts)
    {
        Name = name;
        FlavorText = flavortext;
        Species = species;
        Age = age;
        Sex = sex;
        Voice = voice;
        Gender = gender;
        Appearance = appearance;
        SpawnPriority = spawnPriority;
        _jobPriorities = jobPriorities;
        PreferenceUnavailable = preferenceUnavailable;
        _antagPreferences = antagPreferences;
        _entityTraitPreferences = entityTraitPreferences; // DEN
        // _loadouts = loadouts; - DEN
        _loadoutCategories = loadoutCategories;
        _loadoutProfiles = loadoutProfiles; // DEN
        _jobLoadouts = jobLoadouts; // DEN

        var hasHighPrority = false;
        foreach (var (key, value) in _jobPriorities)
        {
            if (value == JobPriority.Never)
                _jobPriorities.Remove(key);
            else if (value != JobPriority.High)
                continue;

            if (hasHighPrority)
                _jobPriorities[key] = JobPriority.Medium;

            hasHighPrority = true;
        }
    }

    [PublicAPI]
    public HumanoidCharacterProfile WithEntityTraitPreference(ProtoId<EntityTraitPrototype> traitId, IPrototypeManager protoManager)
    {
        // null category is assumed to be default.
        if (!protoManager.TryIndex(traitId, out var traitProto))
            return new(this);

        var category = traitProto.Category;

        // Category not found so dump it.
        TraitCategoryPrototype? traitCategory = null;
        if (category != null && !protoManager.Resolve(category, out traitCategory))
            return new(this);

        var list = new HashSet<ProtoId<EntityTraitPrototype>>(_entityTraitPreferences) { traitId };

        if (traitCategory == null || traitCategory.MaxTraitPoints < 0)
        {
            return new(this)
            {
                _entityTraitPreferences = list,
            };
        }

        var count = 0;
        foreach (var trait in list)
        {
            if (!protoManager.TryIndex(trait, out var otherProto) ||
                otherProto.Category != traitCategory)
                continue;

            count += otherProto.Cost;
        }

        if (count > traitCategory.MaxTraitPoints && traitProto.Cost != 0)
            return new(this);

        return new(this)
        {
            _entityTraitPreferences = list,
        };
    }

    [PublicAPI]
    public HumanoidCharacterProfile WithoutEntityTraitPreference(ProtoId<EntityTraitPrototype> traitId, IPrototypeManager protoManager)
    {
        var list = new HashSet<ProtoId<EntityTraitPrototype>>(_entityTraitPreferences);
        list.Remove(traitId);

        return new(this)
        {
            _entityTraitPreferences = list,
        };
    }

    public HumanoidCharacterProfile WithLoadoutProfile(Guid loadoutId, Guid categoryId, DenLoadoutProfile loadoutProfile)
    {
        if (!_loadoutCategories.TryGetValue(categoryId, out var loadoutCategory))
            return this;

        var categories = new Dictionary<Guid, LoadoutProfileCategory>(_loadoutCategories);

        if (_loadoutProfiles.TryGetValue(loadoutId, out var profile)
            && profile.LoadoutCategory != categoryId)
        {
            var category = categories[profile.LoadoutCategory];
            category.Members.Remove(loadoutId);
            categories[profile.LoadoutCategory] = category;
        }

        var profiles = new Dictionary<Guid, DenLoadoutProfile>(_loadoutProfiles)
        {
            [loadoutId] = loadoutProfile,
        };

        loadoutCategory.Members.Add(loadoutId);
        categories[categoryId] = loadoutCategory;

        return new HumanoidCharacterProfile(this)
        {
            _loadoutProfiles = profiles,
            _loadoutCategories = categories,
        };
    }

    public HumanoidCharacterProfile WithoutLoadoutProfile(DenLoadoutProfile loadoutProfile)
    {
        if (!_loadoutCategories.TryGetValue(loadoutProfile.LoadoutCategory, out var loadoutCategory))
            return this;

        var profiles = new Dictionary<Guid, DenLoadoutProfile>(_loadoutProfiles);
        profiles.Remove(loadoutProfile.Id);

        var categories = new Dictionary<Guid, LoadoutProfileCategory>(_loadoutCategories);
        loadoutCategory.Members.Remove(loadoutProfile.Id);

        categories[loadoutProfile.LoadoutCategory] = loadoutCategory;

        return new HumanoidCharacterProfile(this)
        {
            _loadoutProfiles = profiles,
            _loadoutCategories = categories,
        };
    }

    public HumanoidCharacterProfile WithLoadoutCategory(Guid categoryId, LoadoutProfileCategory profileCategory)
    {
        var categories = new Dictionary<Guid, LoadoutProfileCategory>(_loadoutCategories)
        {
            [categoryId] = profileCategory,
        };

        return new HumanoidCharacterProfile(this)
        {
            _loadoutCategories = categories
        };
    }

    public HumanoidCharacterProfile WithoutLoadoutCategory(LoadoutProfileCategory profileCategory)
    {
        var categories = new Dictionary<Guid, LoadoutProfileCategory>(_loadoutCategories);
        categories.Remove(profileCategory.Id);

        return new HumanoidCharacterProfile(this)
        {
            _loadoutCategories = categories,
        };
    }

    public HumanoidCharacterProfile WithJobLoadout(ProtoId<JobPrototype> jobId, Guid loadoutId)
    {
        var jobLoadouts = new Dictionary<ProtoId<JobPrototype>, HashSet<Guid>>(_jobLoadouts);

        if (!jobLoadouts.ContainsKey(jobId))
            jobLoadouts.Add(jobId, new());

        jobLoadouts[jobId].Add(loadoutId);

        return new HumanoidCharacterProfile(this)
        {
            _jobLoadouts = jobLoadouts
        };
    }

    public HumanoidCharacterProfile WithoutJobLoadout(ProtoId<JobPrototype> jobId, Guid loadoutId)
    {
        var jobLoadouts = new Dictionary<ProtoId<JobPrototype>, HashSet<Guid>>(_jobLoadouts);

        if (!jobLoadouts.ContainsKey(jobId))
            return this;

        jobLoadouts[jobId].Remove(loadoutId);

        return new HumanoidCharacterProfile(this)
        {
            _jobLoadouts = jobLoadouts
        };
    }

    public HumanoidCharacterProfile WithJobLoadouts(ProtoId<JobPrototype> jobId, HashSet<Guid> loadoutIds)
    {
        var jobLoadouts = new Dictionary<ProtoId<JobPrototype>, HashSet<Guid>>(_jobLoadouts)
        {
            [jobId] = new(loadoutIds),
        };

        return new HumanoidCharacterProfile(this)
        {
            _jobLoadouts = jobLoadouts
        };
    }

    /// <summary>
    /// Takes in an IEnumerable of traits and returns a List of the valid traits.
    /// </summary>
    public List<ProtoId<EntityTraitPrototype>> GetValidEntityTraits(IEnumerable<ProtoId<EntityTraitPrototype>> traits,
        IPrototypeManager protoManager)
    {
        // Track points count for each group.
        var groups = new Dictionary<string, int>();
        var result = new List<ProtoId<EntityTraitPrototype>>();

        foreach (var trait in traits)
        {
            if (!protoManager.TryIndex(trait, out var traitProto))
                continue;

            // Always valid.
            if (traitProto.Category == null)
            {
                result.Add(trait);
                continue;
            }

            // No category so dump it.
            if (!protoManager.Resolve(traitProto.Category, out var category))
                continue;

            var existing = groups.GetOrNew(category.ID);
            existing += traitProto.Cost;

            // Too expensive.
            if (existing > category.MaxTraitPoints)
                continue;

            groups[category.ID] = existing;
            result.Add(trait);
        }

        return result;
    }

}
