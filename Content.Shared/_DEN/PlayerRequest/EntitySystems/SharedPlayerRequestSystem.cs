using Content.Shared._DEN.PlayerRequest.Components;
using Content.Shared._DEN.PlayerRequest.Events;
using Content.Shared.Alert;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.EntitySystems;

/// <summary>
/// Handles player-to-player requests, such as when a player offers something.
/// </summary>
public abstract partial class SharedPlayerRequestSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alertsSystem = null!;
    [Dependency] private readonly SharedMindSystem _mindSystem = null!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = null!;
    [Dependency] private readonly IPrototypeManager _protoManager = null!;

    private const PopupType RequestPopupType = PopupType.Medium;
    private readonly Dictionary<ProtoId<AlertPrototype>, ProtoId<PlayerRequestPrototype>> _alertTranslations = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AcceptedRequestAlertEvent>(OnRequestAcceptedAlert);
        SubscribeLocalEvent<PlayerRequestStartedEvent>(OnRequestStarted);
        SubscribeLocalEvent<PlayerRequestUpdatedEvent>(OnRequestUpdated);

        InitializePrototypes();
        _protoManager.PrototypesReloaded += OnPrototypesReloaded;
    }

    private void OnRequestStarted(PlayerRequestStartedEvent ev)
    {
        if (!_protoManager.TryIndex(ev.RequestId, out var playerRequest))
            return;

        var receivePopup = Loc.GetString(playerRequest.ReceivePopup, ("sender", ev.Sender));
        var sendPopup = Loc.GetString(playerRequest.SendPopup, ("receiver", ev.Receiver));

        ShowAlert(ev.RequestId, ev.Receiver);

        _popupSystem.PopupPredicted(receivePopup, ev.Receiver, ev.Receiver, RequestPopupType);
        _popupSystem.PopupPredicted(sendPopup, ev.Sender, ev.Sender, RequestPopupType);
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<PlayerRequestPrototype>())
            InitializePrototypes();
    }

    private void OnRequestAcceptedAlert(AcceptedRequestAlertEvent ev)
    {
        if (!_alertTranslations.TryGetValue(ev.AlertId, out var requestId))
            return;

        var statusUpdatedEvent = new PlayerRequestUpdatedEvent(requestId, ev.User, true);
        RaiseLocalEvent(statusUpdatedEvent);
    }

    private void OnRequestUpdated(PlayerRequestUpdatedEvent ev)
    {
        if (!ev.IsApproved)
            return;

        var requestProto = _protoManager.Index(ev.RequestId);

        if (!TryComp<RequestReceiverComponent>(ev.Receiver, out var requestComp))
            return;

        if (!TryGetSender(ev.RequestId, ev.Receiver, out var sender, requestComp))
            return;

        ClearAlert(ev.RequestId, ev.Receiver);

        if (requestProto.AcceptPopup == null)
            return;

        var acceptPopup = Loc.GetString(requestProto.AcceptPopup, ("receiver", ev.Receiver));
        _popupSystem.PopupPredicted(acceptPopup, sender, sender, RequestPopupType);
    }
}

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
