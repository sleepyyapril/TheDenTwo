using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Consent.Events;

public sealed class ConsentUpdatedEventArgs(
    NetUserId userId,
    ProtoId<ConsentTogglePrototype> toggleId,
    bool newValue) : EventArgs
{
    public readonly NetUserId UserId = userId;
    public readonly ProtoId<ConsentTogglePrototype> ToggleId = toggleId;
    public readonly bool ToggleValue = newValue;
}
