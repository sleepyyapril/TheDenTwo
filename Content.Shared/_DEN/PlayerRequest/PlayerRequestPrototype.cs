using Content.Shared.Alert;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared._DEN.PlayerRequest;

/// <summary>
/// This is a prototype for declaring player requests
/// </summary>
[Prototype]
public sealed class PlayerRequestPrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<PlayerRequestPrototype>))]
    public string[]? Parents { get; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    /// <summary>
    /// The specific alert to use for this request.
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> Alert = "PlayerRequestAcceptAlert";

    /// <summary>
    /// How long (in seconds) should the request stay waiting for a player to accept before canceling?
    /// </summary>
    [DataField]
    public int AutoDeclineAfter { get; } = 30;

    /// <summary>
    /// The popup message that will appear when a player receives this request.
    /// </summary>
    [DataField(required: true)]
    public LocId ReceivePopup { get; }

    /// <summary>
    /// The popup message that will appear when a player sends the request.
    /// </summary>
    [DataField(required: true)]
    public LocId SendPopup { get; }

    /// <summary>
    /// The popup message that will appear to the requester when the target player accepts this request.
    /// </summary>
    [DataField]
    public LocId? AcceptPopup { get; }

    /// <summary>
    /// The popup message that will appear to the requester when the target player accepts this request.
    /// </summary>
    [DataField]
    public LocId? CancelledPopup { get; }
}
