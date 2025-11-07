using Content.Shared._Funkystation.Material;
using Robust.Shared.Prototypes;


namespace Content.Shared._Funkystation.ResourceOverview.Prototypes;


/// <summary>
/// This is a prototype that keeps track of what each
/// </summary>
[Prototype]
public sealed partial class LogisticsEntryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public Dictionary<ProtoId<MaterialCategoryPrototype>, Dictionary<ResourceStatus, int>> Thresholds { get; } = new();
}
