using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.Components;

/// <summary>
/// This is used for tracking active requests where this user is the sender.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RequestSenderComponent : Component
{
    /// <summary>
    /// The other user involved in a player request.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<PlayerRequestPrototype>, EntityUid> Receivers = new();
}
