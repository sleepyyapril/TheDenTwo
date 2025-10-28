using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Consent.Prototypes;

/// <summary>
/// This is a prototype for declaring consent toggles.
/// </summary>
[Prototype]
public sealed partial class ConsentTogglePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public bool DefaultValue { get; set; }
}
