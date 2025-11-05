using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.Components;

/// <summary>
/// This is used for tracking active requests where the user is a receiver.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RequestReceiverComponent : Component
{
    /// <summary>
    /// The other users involved in a player request.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<PlayerRequestPrototype>, EntityUid> Senders = new();
}
