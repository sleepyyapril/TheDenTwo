using Content.Server.Popups;
using Content.Shared._DEN.PlayerRequest;
using Content.Shared._DEN.PlayerRequest.Components;
using Content.Shared._DEN.PlayerRequest.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Server._DEN.PlayerRequest;

public sealed class PlayerRequestSystem : SharedPlayerRequestSystem
{
    [Dependency] private readonly PopupSystem _popupSystem = null!;

    public override void StartRequest(ProtoId<PlayerRequestPrototype> request, EntityUid sender, EntityUid receiver)
    {
        if (!CanRequest(request, sender, receiver, out var reason))
        {
            _popupSystem.PopupEntity(reason, sender, sender, PopupType.MediumCaution);
            return;
        }

        var ev = new AttemptPlayerRequestEvent(request, sender, receiver);
        RaiseLocalEvent(ref ev);

        if (ev.Cancelled)
            return;

        var senderComp = EnsureComp<RequestSenderComponent>(sender);
        var receiverComp = EnsureComp<RequestReceiverComponent>(receiver);

        senderComp.Receivers[request] = receiver;
        receiverComp.Senders[request] = sender;

        Dirty(sender, senderComp);
        Dirty(receiver, receiverComp);

        RaiseLocalEvent(new PlayerRequestStartedEvent(request, sender, receiver));
    }

    public override void ApproveRequest(ProtoId<PlayerRequestPrototype> request, EntityUid receiver)
    {
        var statusUpdatedEvent = new PlayerRequestUpdatedEvent(request, receiver, true);
        RaiseLocalEvent(statusUpdatedEvent);
    }

    public override void CancelRequest(ProtoId<PlayerRequestPrototype> request, EntityUid cancelling)
    {
        var statusUpdatedEvent = new PlayerRequestUpdatedEvent(request, cancelling, false);
        RaiseLocalEvent(statusUpdatedEvent);
    }
}
