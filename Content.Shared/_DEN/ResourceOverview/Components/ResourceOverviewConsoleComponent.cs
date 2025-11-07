using Content.Shared._Funkystation.ResourceOverview.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;


namespace Content.Shared._Funkystation.ResourceOverview.Components;

/// <summary>
///     Marks an entity as a Resource Overview Console.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedResourceOverviewConsoleSystem))]
public sealed partial class ResourceOverviewConsoleComponent : Component
{
    [ViewVariables]
    public EntityUid? Focus { get; set; }
}

[Serializable, NetSerializable]
public record struct ResourceOverviewEntry
{
    /// <summary>
    /// The focus entity
    /// </summary>
    public NetEntity FocusEntity;

    /// <summary>
    /// The resource count of the focus entity
    /// </summary>
    public Dictionary<string, int> Resources;
}

[Serializable, NetSerializable]
public sealed class ResourceOverviewBoundInterfaceState : BoundUserInterfaceState
{
    /// <summary>
    /// An updated list of entries, usually lathes.
    /// </summary>
    public HashSet<ResourceOverviewEntry> Entries;

    public ResourceOverviewBoundInterfaceState(HashSet<ResourceOverviewEntry> entries)
    {
        Entries = entries;
    }
}

[Serializable, NetSerializable]
public enum ResourceOverviewConsoleUiKey
{
    Key
}
