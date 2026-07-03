using Robust.Shared.Prototypes;

namespace Content.Server._DEN.Loadout.Modules;

[Serializable]
public sealed class DescribableModule : ILoadoutModule
{
    /// <summary>
    /// The name of the item as per examine.
    /// </summary>
    public string? CustomName { get; set; }

    /// <summary>
    /// The description of the item as per examine.
    /// </summary>
    public string? CustomDescription { get; set; }

    public void OnEntitySpawn(IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityUid uid)
    {
        if (!entMan.TryGetComponent<MetaDataComponent>(uid, out var metaData))
            return;

        if (string.IsNullOrWhiteSpace(CustomName) && string.IsNullOrWhiteSpace(CustomDescription))
            return;

        var metaDataSystem = entMan.System<MetaDataSystem>();

        if (!string.IsNullOrEmpty(CustomName))
            metaDataSystem.SetEntityName(uid, CustomName, metaData);

        if (!string.IsNullOrEmpty(CustomDescription))
            metaDataSystem.SetEntityDescription(uid, CustomDescription, metaData);
    }
}
