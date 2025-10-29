using Content.Shared._DEN.Consent.EntitySystems;
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
    event Action<NetUserId>? OnConsentSet;

    void Initialize();
    void SetConsentToggle(NetUserId userId, ProtoId<ConsentTogglePrototype> toggle, bool newValue);
    void SetConsentToggles(NetUserId userId, List<UserConsentInfo> toggles);
    List<ProtoId<ConsentTogglePrototype>> GetConsentToggles(NetUserId userId);
    bool GetDefaultValue(ProtoId<ConsentTogglePrototype> toggle);
}

