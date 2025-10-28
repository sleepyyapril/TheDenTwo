using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Consent.Components;

/// <summary>
/// This is used for sharing a user's consent information with other clients.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ConsentComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<ConsentTogglePrototype>, bool> ConsentToggles { get; set; } = new();
}
