using Robust.Shared.GameStates;

namespace Content.Shared._DEN.PlayerRequest;

/// <summary>
/// This is used for tracking an active request.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlayerRequestComponent : Component
{
    /// <summary>
    /// Whether the user is the requester or not.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsRequester;

    /// <summary>
    /// The other user involved in a player request.
    /// </summary>
    [DataField, AutoNetworkedField]
    public NetEntity Target;
}
