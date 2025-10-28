using System.Linq;
using System.Threading;
using Content.Shared._DEN.Consent.Events;
using Content.Shared._DEN.Consent.Prototypes;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._DEN.Consent.Managers;

/// <summary>
/// Used to store consent information.
/// Only stores values that are not the default value of the toggle.
/// </summary>
public abstract class ConsentManager : IConsentManager
{
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    [ViewVariables]
    protected Dictionary<NetUserId, List<ProtoId<ConsentTogglePrototype>>> InternalConsents { get; } = new();
    protected Dictionary<ProtoId<ConsentTogglePrototype>, bool> DefaultToggleValues { get; } = new();

    protected readonly ReaderWriterLockSlim Lock = new();

    public Dictionary<NetUserId, List<ProtoId<ConsentTogglePrototype>>> UserConsents
    {
        get
        {
            Lock.EnterReadLock();
            try
            {
                return InternalConsents.ShallowClone();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }
    }

    public event Action<ConsentUpdatedEventArgs>? OnConsentUpdated;
    public event Action? OnConsentSet;

    public void Initialize()
    {
        _protoManager.PrototypesReloaded += OnPrototypesReloaded;
        CacheDefaultToggleValues();
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<ConsentTogglePrototype>())
            CacheDefaultToggleValues();
    }

    private void CacheDefaultToggleValues()
    {
        DefaultToggleValues.Clear();

        foreach (var toggle in _protoManager.EnumeratePrototypes<ConsentTogglePrototype>())
        {
            DefaultToggleValues[toggle] = toggle.DefaultValue;
        }
    }

    public void SetConsentToggle(NetUserId userId, ProtoId<ConsentTogglePrototype> toggle, bool newValue)
    {
        var toggles = GetConsentToggles(userId);

        if (newValue == DefaultToggleValues[toggle])
            toggles.Remove(toggle);
        else
            AddConsentToggle(ref toggles, toggle);

        InternalConsents[userId] = toggles;

        var updatedEvent = new ConsentUpdatedEventArgs(userId, toggle, newValue);
        OnConsentUpdated?.Invoke(updatedEvent);
    }

    public void SetConsentToggles(NetUserId userId, List<ProtoId<ConsentTogglePrototype>> toggles)
    {
        InternalConsents[userId] = toggles;
        OnConsentSet?.Invoke();
    }

    public List<ProtoId<ConsentTogglePrototype>> GetConsentToggles(NetUserId userId)
    {
        var exists = UserConsents.TryGetValue(userId, out var consentToggles);

        if (!exists || consentToggles == null)
            return [];

        return consentToggles;
    }

    private void AddConsentToggle(ref List<ProtoId<ConsentTogglePrototype>> toggles,
        ProtoId<ConsentTogglePrototype> toggle)
    {
        if (toggles.Contains(toggle))
            return;

        toggles.Add(toggle);
    }
}
