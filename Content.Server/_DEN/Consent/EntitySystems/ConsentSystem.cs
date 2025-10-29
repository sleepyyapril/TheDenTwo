using Content.Shared._DEN.Consent.Components;
using Content.Shared._DEN.Consent.EntitySystems;
using Content.Shared._DEN.Consent.Events;
using Content.Shared._DEN.Consent.Managers;
using Content.Shared.Mind.Components;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._DEN.Consent.EntitySystems;

/// <summary>
/// This handles server-sided consent information.
/// </summary>
public sealed class ConsentSystem : SharedConsentSystem
{
    [Dependency] private readonly IConsentManager _consentManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerAttachedEvent>(OnMindAdded);
        SubscribeLocalEvent<PlayerDetachedEvent>(OnMindRemoved);

        _consentManager.OnConsentUpdated += args => OnConsentUpdated(args.UserId);
        _consentManager.OnConsentSet += OnConsentUpdated;
    }

    private void OnConsentUpdated(NetUserId userId)
    {
        if (!_playerManager.TryGetSessionById(userId, out var session)
            || session.AttachedEntity is not { Valid: true } attachedEntity)
            return;

        var consentToggles = ConsentManager.GetConsentToggles(userId);
        var consentComponent = EnsureComp<ConsentComponent>(attachedEntity);

        if (consentComponent.ConsentToggles == consentToggles)
            return;

        consentComponent.ConsentToggles = consentToggles;
        Dirty<ConsentComponent>((attachedEntity, consentComponent));
    }

    private void OnMindAdded(PlayerAttachedEvent ev)
    {
        BuildConsentComponent(ev);
    }

    private void OnMindRemoved(PlayerDetachedEvent ev)
    {
        RemComp<ConsentComponent>(ev.Entity);
    }

    private void BuildConsentComponent(PlayerAttachedEvent ev)
    {
        var userId = ev.Player.UserId;

        var consentToggles = ConsentManager.GetConsentToggles(userId);
        var consentComponent = EnsureComp<ConsentComponent>(ev.Entity);
        consentComponent.ConsentToggles = consentToggles;

        Dirty<ConsentComponent>((ev.Entity, consentComponent));
    }
}
