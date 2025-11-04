using Content.Server.Alert;
using Content.Server.Popups;
using Content.Shared._DEN.PlayerRequest;
using Content.Shared.Alert;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Server._DEN.PlayerRequest;

// todo: prediction??? maybe unnecessary???
public sealed class PlayerRequestSystem : SharedPlayerRequestSystem
{
    [Dependency] private readonly ServerAlertsSystem _alertsSystem = null!;
    [Dependency] private readonly PopupSystem _popupSystem = null!;
    [Dependency] private readonly IPrototypeManager _protoManager = null!;

    private readonly Dictionary<ProtoId<PlayerRequestPrototype>, List<RequestRegistryEntry>> _registryEntries = new();
    private readonly Dictionary<ProtoId<AlertPrototype>, ProtoId<PlayerRequestPrototype>> _alertTranslations = new();

    private const PopupType RequestPopupType = PopupType.Medium;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AcceptedRequestAlertEvent>(OnRequestAccepted);

        _protoManager.PrototypesReloaded += OnPrototypesReloaded;
        InitializePrototypes();
    }

    private void OnRequestAccepted(AcceptedRequestAlertEvent ev)
    {
        var translation = _alertTranslations[ev.AlertId];
        var requestProto = _protoManager.Index(translation);

        if (!_registryEntries.TryGetValue(requestProto, out var events)
            || events.Count == 0)
            return;

        // This should never happen.
        if (!TryComp<PlayerRequestComponent>(ev.User, out var requestComp))
            throw new InvalidOperationException("Request was accepted, but we don't have a PlayerRequestComponent?");

        var requesterUid = GetEntity(requestComp.Target);

        if (requesterUid == EntityUid.Invalid)
            return;

        var statusUpdatedEvent = new PlayerRequestStatusUpdatedEvent(translation, ev.User, true);
        RaiseLocalEvent(statusUpdatedEvent);

        foreach (var entry in events)
        {
            var requester = requestComp.IsRequester ? ev.User : requesterUid;
            var target = !requestComp.IsRequester ? ev.User : requesterUid;

            entry.OnAccepted(requester, target);
            ClearAlert(translation, target);

            RemCompDeferred<PlayerRequestComponent>(requester);
            RemCompDeferred<PlayerRequestComponent>(target);

            if (requestProto.AcceptPopup == null)
                continue;

            var acceptPopup = Loc.GetString(requestProto.AcceptPopup, ("target", target));
            _popupSystem.PopupEntity(acceptPopup, requester, requester, RequestPopupType);
        }
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<PlayerRequestPrototype>())
            InitializePrototypes();
    }

    /// <summary>
    /// Registers a new <see cref="RequestRegistryEntry"/> for player request callbacks.
    /// </summary>
    /// <param name="request">The <see cref="PlayerRequestPrototype"/> that the request falls under.</param>
    /// <param name="onAccepted">The callback to run if they accepted the request.</param>
    /// <param name="onCancelled">The optional callback to run if they declined the request.</param>
    /// <remarks>Parameters for callback actions are always ordered: Requester, Target</remarks>
    public void RegisterRequestCallback(ProtoId<PlayerRequestPrototype> request,
        Action<EntityUid, EntityUid> onAccepted,
        Action<EntityUid, EntityUid>? onCancelled = null
        )
    {
        if (!_registryEntries.TryGetValue(request, out _))
            _registryEntries[request] = new List<RequestRegistryEntry>();

        var requestInfo = new RequestRegistryEntry(onAccepted, onCancelled);
        _registryEntries[request].Add(requestInfo);
    }

    public override void StartRequest(ProtoId<PlayerRequestPrototype> request, EntityUid requester, EntityUid target)
    {
        if (HasComp<PlayerRequestComponent>(requester) || HasComp<PlayerRequestComponent>(target)
            || !_protoManager.TryIndex(request, out var requestProto))
            return;

        var ev = new AttemptPlayerRequestEvent(request, requester, target);
        RaiseLocalEvent(ref ev);

        if (ev.Cancelled)
            return;

        var requesterComp = EnsureComp<PlayerRequestComponent>(requester);
        var targetComp = EnsureComp<PlayerRequestComponent>(target);

        var receivePopup = Loc.GetString(requestProto.ReceivePopup, ("requester", requester));
        var sendPopup = Loc.GetString(requestProto.SendPopup, ("target", target));

        requesterComp.IsRequester = true;
        requesterComp.Target = GetNetEntity(target);

        targetComp.IsRequester = false;
        targetComp.Target = GetNetEntity(requester);

        Dirty(requester, requesterComp);
        Dirty(target, targetComp);
        ShowAlert(request, target);

        _popupSystem.PopupEntity(receivePopup, target, target, RequestPopupType);
        _popupSystem.PopupEntity(sendPopup, requester, requester, RequestPopupType);

        RaiseLocalEvent(new PlayerRequestStartedEvent(request, requester, target));
    }

    private void ShowAlert(ProtoId<PlayerRequestPrototype> request, EntityUid target)
    {
        if (!Exists(target) || !TryComp<AlertsComponent>(target, out var alerts))
            return;

        if (!_protoManager.TryIndex(request, out var requestProto))
            return;

        _alertsSystem.ShowAlert((target, alerts), requestProto.Alert);
    }

    private void ClearAlert(ProtoId<PlayerRequestPrototype> request, EntityUid target)
    {
        if (!Exists(target) || !TryComp<AlertsComponent>(target, out var alerts))
            return;

        if (!_protoManager.TryIndex(request, out var requestProto))
            return;

        _alertsSystem.ClearAlert((target, alerts), requestProto.Alert);
    }

    private void InitializePrototypes()
    {
        foreach (var requestPrototype in _protoManager.EnumeratePrototypes<PlayerRequestPrototype>())
        {
            _alertTranslations[requestPrototype.Alert] = requestPrototype;

            if (!_registryEntries.TryGetValue(requestPrototype, out var registry))
                registry = new();

            _registryEntries[requestPrototype] = registry;
        }
    }
}

[ByRefEvent]
public record struct AttemptPlayerRequestEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid Requester,
    EntityUid Target,
    bool Cancelled = false);

public record struct PlayerRequestStartedEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid Requester,
    EntityUid Target);

public record struct PlayerRequestStatusUpdatedEvent(
    ProtoId<PlayerRequestPrototype> RequestId,
    EntityUid UpdatedBy,
    bool IsApproved);

public record struct RequestRegistryEntry(
    Action<EntityUid, EntityUid> OnAccepted,
    Action<EntityUid, EntityUid>? OnCancelled);
