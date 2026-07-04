using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Loadout;

/// <summary>
/// This is a prototype for defining loadout categories.
/// </summary>
[Prototype]
public sealed partial class LoadoutCategoryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField, ViewVariables]
    public string ID { get; private set;  } = default!;

    [DataField]
    public LocId Name = string.Empty;

    [DataField]
    public bool Root;

    [DataField]
    public HashSet<ProtoId<LoadoutCategoryPrototype>> SubCategories = new();

    [DataField]
    public int Priority;

    [DataField]
    public int MaxItems;
}
