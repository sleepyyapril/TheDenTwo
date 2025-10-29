using Content.Shared._DEN.Consent.Components;
using Content.Shared._DEN.Consent.Managers;
using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Consent.EntitySystems;

public abstract class SharedConsentSystem : EntitySystem
{
    [Dependency] protected readonly IConsentManager ConsentManager = default!;

    public bool HasConsent(EntityUid uid, ProtoId<ConsentTogglePrototype> toggle)
    {
        var defaultValue = ConsentManager.GetDefaultValue(toggle);

        if (TryComp<ConsentComponent>(uid, out var consent)
            && consent.ConsentToggles.Contains(toggle))
            return !defaultValue;

        return defaultValue;
    }
}

public record struct UserConsentInfo(ProtoId<ConsentTogglePrototype> ToggleId, bool ToggleValue);
