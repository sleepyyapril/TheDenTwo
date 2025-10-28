using Content.Shared._DEN.Consent.Events;
using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Consent.Managers;

/// <summary>
/// Handle player consent information
/// </summary>
public interface IConsentManager
{
    event Action<ConsentUpdatedEventArgs>? OnConsentUpdated;
    event Action? OnConsentSet;
    protected Dictionary<NetUserId, List<ProtoId<ConsentTogglePrototype>>> UserConsents { get; }


    void SetConsentToggle(NetUserId userId, ProtoId<ConsentTogglePrototype> toggle, bool newValue);
    void SetConsentToggles(NetUserId userId, List<ProtoId<ConsentTogglePrototype>> toggles);
    List<ProtoId<ConsentTogglePrototype>> GetConsentToggles(NetUserId userId);
}

