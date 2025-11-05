using Content.Shared._DEN.PlayerRequest.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.EntitySystems;

public abstract partial class SharedPlayerRequestSystem
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
    /// <param name="cancelling">The user canceling the request. <see cref="RequestSenderComponent"/></param>
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
    /// <param name="receiver">The receiver who approved the request.</param>
    public virtual void ApproveRequest(
        ProtoId<PlayerRequestPrototype> request,
        EntityUid receiver)
    {
    }

    /// <summary>
    /// Tries to get the sender's <see cref="EntityUid"/> from a receiver <see cref="EntityUid"/>.
    /// </summary>
    /// <param name="request">The request that we should try to find the sender in.</param>
    /// <param name="receiver">The receiver that received the request.</param>
    /// <param name="sender">The output sender that was acquired.</param>
    /// <param name="receiverComp">The <see cref="RequestReceiverComponent"/> to be resolved.</param>
    /// <returns>True if a valid EntityUid was found, false if not.</returns>
    /// <remarks>The entity will always exist if it returned true.</remarks>
    public bool TryGetSender(ProtoId<PlayerRequestPrototype> request,
        EntityUid receiver,
        out EntityUid sender,
        RequestReceiverComponent? receiverComp = null)
    {
        sender = EntityUid.Invalid;

        if (!Resolve(receiver, ref receiverComp, false)
            || !receiverComp.Senders.TryGetValue(request, out var potentialSender)
            || !Exists(potentialSender))
            return false;

        sender = potentialSender;
        return true;
    }

    /// <summary>
    /// Tries to get the receiver's <see cref="EntityUid"/> from a sender <see cref="EntityUid"/>.
    /// </summary>
    /// <param name="request">The request that the sender sent.</param>
    /// <param name="sender">The sender that sent the request.</param>
    /// <param name="receiver">The output receiver acquired.</param>
    /// <param name="senderComp">The <see cref="RequestSenderComponent"/> to be resolved.</param>
    /// <returns>True if a valid EntityUid was found, false if not.</returns>
    /// <remarks>The entity will always exist if it returned true.</remarks>
    public bool TryGetReceiver(ProtoId<PlayerRequestPrototype> request,
        EntityUid sender,
        out EntityUid receiver,
        RequestSenderComponent? senderComp = null)
    {
        receiver = EntityUid.Invalid;

        if (!Resolve(sender, ref senderComp, false)
            || !senderComp.Receivers.TryGetValue(request, out var potentialReceiver)
            || !Exists(potentialReceiver))
            return false;

        receiver = potentialReceiver;
        return true;
    }

    /// <summary>
    /// Whether or not a request should go forth.
    /// </summary>
    /// <param name="request">The request attempting to be made.</param>
    /// <param name="sender">The sender trying to send a request.</param>
    /// <param name="receiver">The receiver that would get the request.</param>
    /// <param name="reason">The display-friendly message if we couldn't request.</param>
    /// <returns>Whether or not they can request in their current condition.</returns>
    protected bool CanRequest(
        ProtoId<PlayerRequestPrototype> request,
        EntityUid sender,
        EntityUid receiver,
        out string? reason)
    {
        reason = null;

        // If any of the users don't exist or don't have a mind,
        // there's no one to send a request to.
        if (!Exists(sender) || !Exists(receiver)
                            || !_mindSystem.TryGetMind(sender, out _, out _)
                            || !_mindSystem.TryGetMind(receiver, out _, out _))
        {
            return false;
        }

        // Alerts only support one alert at a time, so might as well enforce it.
        if (TryComp<RequestSenderComponent>(sender, out var requestSenderComp)
            && requestSenderComp.Receivers.ContainsKey(request))
        {
            reason = Loc.GetString("player-request-existing-sender", ("sender", sender));
            return false;
        }

        if (TryComp<RequestReceiverComponent>(receiver, out var requestReceiverComp)
            && requestReceiverComp.Senders.ContainsKey(request))
        {
            reason = Loc.GetString("player-request-existing-receiver", ("receiver", receiver));
            return false;
        }

        return true;
    }
}
