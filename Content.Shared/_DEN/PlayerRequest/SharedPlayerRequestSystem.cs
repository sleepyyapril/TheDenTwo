using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest;

/// <summary>
/// Handles player-to-player requests, such as when a player offers something.
/// </summary>
public abstract partial class SharedPlayerRequestSystem : EntitySystem
{
    /// <summary>
    /// Start a player request that sends the popups and alert as needed.
    /// </summary>
    /// <param name="request">The <see cref="PlayerRequestPrototype"/> ID for this request.</param>
    /// <param name="requester">The one who initiated the request.</param>
    /// <param name="target">The target of the request.</param>
    public virtual void StartRequest(ProtoId<PlayerRequestPrototype> request,
        EntityUid requester,
        EntityUid target)
    {
    }

    /// <summary>
    /// Cancels a player request, removing the alert and notifying the other party.
    /// Sends a PlayerRequestStatusUpdated event.
    /// </summary>
    /// <param name="request">The <see cref="PlayerRequestPrototype"/> ID for this request.</param>
    /// <param name="cancelling">The user canceling the request. <see cref="PlayerRequestComponent"/></param>
    public virtual void CancelRequest(
        ProtoId<PlayerRequestPrototype> request,
        EntityUid cancelling)
    {
    }

    /// <summary>
    /// Approves a player request, removing the alert.
    /// <see cref="PlayerRequestPrototype"/> also allows for an optional popup to the requester when approved.
    /// Sends a PlayerRequestStatusUpdated event.
    /// </summary>
    /// <param name="request">The <see cref="PlayerRequestPrototype"/> ID for this request.</param>
    /// <param name="target">The target who approved the request.</param>
    public virtual void ApproveRequest(
        ProtoId<PlayerRequestPrototype> request,
        EntityUid target)
    {
    }
}
