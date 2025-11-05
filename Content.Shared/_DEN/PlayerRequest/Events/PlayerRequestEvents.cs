using Content.Shared.Alert;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.Events;

[ByRefEvent]
public record struct AttemptPlayerRequestEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid Sender,
    EntityUid Receiver,
    bool Cancelled = false);

public record struct PlayerRequestStartedEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid Sender,
    EntityUid Receiver);

public record struct PlayerRequestUpdatedEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid Receiver,
    bool IsApproved);

public sealed partial class AcceptedRequestAlertEvent : BaseAlertEvent;
