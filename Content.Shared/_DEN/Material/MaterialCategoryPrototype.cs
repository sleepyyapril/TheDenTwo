using Content.Shared.Materials;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Material;

/// <summary>
/// This is a prototype for categorizing materials.
/// </summary>
[Prototype]
public sealed partial class MaterialCategoryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public List<ProtoId<MaterialPrototype>> Materials { get; } = new();
}
