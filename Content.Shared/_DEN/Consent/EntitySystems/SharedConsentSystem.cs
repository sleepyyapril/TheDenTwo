using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Consent.EntitySystems;

/// <summary>
/// This handles updating consent based on entity events.
/// </summary>
public abstract class SharedConsentSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
    }
}

[Serializable, NetSerializable]
public record struct UserConsentToggle(ProtoId<ConsentTogglePrototype> ToggleId);

