using Content.Shared.Alert;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.PlayerRequest.EntitySystems;

public abstract partial class SharedPlayerRequestSystem
{
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
        }
    }
}
