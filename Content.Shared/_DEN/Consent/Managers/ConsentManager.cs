using System.Linq;
using System.Threading;
using Content.Shared._DEN.Consent.EntitySystems;
using Content.Shared._DEN.Consent.Events;
using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._DEN.Consent.Managers;

/// <summary>
/// Used to store consent information.
/// Only stores values that are not the default value of the toggle.
/// </summary>
public sealed class ConsentManager : IConsentManager
{

    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    [ViewVariables]
    private Dictionary<NetUserId, List<UserConsentInfo>> InternalConsents { get; } = new();
    private Dictionary<ProtoId<ConsentTogglePrototype>, bool> DefaultToggleValues { get; } = new();

    public event Action<ConsentUpdatedEventArgs>? OnConsentUpdated;
    public event Action<NetUserId>? OnConsentSet;

    public void Initialize()
    {
        _protoManager.PrototypesReloaded += OnPrototypesReloaded;
        _playerManager.PlayerStatusChanged += (_, args) => OnPlayerStatusChanged(args);
        CacheDefaultToggleValues();
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<ConsentTogglePrototype>())
            CacheDefaultToggleValues();
    }

    private void OnPlayerStatusChanged(SessionStatusEventArgs args)
    {
        var toggles = GetDefaultToggles();

        if (!InternalConsents.TryGetValue(args.Session.UserId, out _))
            InternalConsents[args.Session.UserId] = toggles;
    }

    private void CacheDefaultToggleValues()
    {
        DefaultToggleValues.Clear();

        foreach (var toggle in _protoManager.EnumeratePrototypes<ConsentTogglePrototype>())
        {
            DefaultToggleValues[toggle.ID] = toggle.DefaultValue;
        }
    }

    public void SetConsentToggle(NetUserId userId, ProtoId<ConsentTogglePrototype> toggleId, bool newValue)
    {
        if (!InternalConsents.ContainsKey(userId))
            InternalConsents[userId] = GetDefaultToggles();

        var toggles = GetConsentTogglesExcept(userId, toggleId);
        var toggle = new UserConsentInfo(toggleId, newValue);

        if (newValue != DefaultToggleValues[toggleId])
            toggles.Add(toggle);

        InternalConsents[userId] = toggles;

        var updatedEvent = new ConsentUpdatedEventArgs(userId, toggle.ToggleId, toggle.ToggleValue);
        OnConsentUpdated?.Invoke(updatedEvent);
    }

    public void SetConsentToggles(NetUserId userId, List<UserConsentInfo> toggles)
    {
        InternalConsents[userId] = toggles;
        OnConsentSet?.Invoke(userId);
    }

    public List<ProtoId<ConsentTogglePrototype>> GetConsentToggles(NetUserId userId)
    {
        var exists = InternalConsents.TryGetValue(userId, out var consentToggles);

        if (!exists || consentToggles == null)
            return [];

        var consentIds = new List<ProtoId<ConsentTogglePrototype>>();

        foreach (var toggle in consentToggles)
        {
            if (toggle.ToggleValue == DefaultToggleValues[toggle.ToggleId])
                continue;

            consentIds.Add(toggle.ToggleId);
        }

        return consentIds;
    }

    private List<UserConsentInfo> GetDefaultToggles()
    {
        var defaultToggles = new List<UserConsentInfo>();

        foreach (var pair in DefaultToggleValues)
        {
            var userConsent = new UserConsentInfo(pair.Key, pair.Value);
            defaultToggles.Add(userConsent);
        }

        return defaultToggles;
    }

    private List<UserConsentInfo> GetConsentTogglesExcept(NetUserId userId, ProtoId<ConsentTogglePrototype> toggleId)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!InternalConsents.TryGetValue(userId, out var consentToggles))
            consentToggles = GetDefaultToggles();

        var consentIds = new List<UserConsentInfo>();

        foreach (var toggle in consentToggles)
        {
            if (toggle.ToggleId == toggleId)
                continue;

            consentIds.Add(toggle);
        }

        return consentIds;
    }

    public bool GetDefaultValue(ProtoId<ConsentTogglePrototype> toggle)
    {
        return DefaultToggleValues[toggle];
    }
}
