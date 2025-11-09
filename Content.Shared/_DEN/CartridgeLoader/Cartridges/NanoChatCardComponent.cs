using Robust.Shared.Serialization;

namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NanoChatCardComponent : Component
{
    [DataField]
    public bool ListNumber { get; set; } = true;

    [ViewVariables]
    public EntityUid? LoaderUid { get; set; }

    [ViewVariables]
    public uint? PersonalNumber { get; set; }

    [ViewVariables]
    public Guid? CurrentChat { get; set; }

    [ViewVariables]
    public HashSet<Guid> UnreadMessages { get; set; } = new();

    [ViewVariables]
    public Dictionary<Guid, NanoChatConversation> Conversations { get; private set; } = new();

    [ViewVariables]
    public Dictionary<uint, NanoChatUser> RelevantUsers { get; private set; } = new();

    [ViewVariables]
    public Dictionary<Guid, NanoChatMessage> Messages { get; private set; } = new();
}

[Serializable, NetSerializable]
public record struct NanoChatUser(uint UniqueId, string Name, string Job);

[Serializable, NetSerializable]
public record struct NanoChatConversation(
    Guid ConversationId,
    HashSet<uint> Members,
    HashSet<Guid> Messages,
    NanoChatConversationFlags Flags);

[Serializable, NetSerializable]
public record struct NanoChatMessage(
    NanoChatUser Sender,
    Guid ConversationId,
    NanoChatMessageFlags Flags,
    NanoChatMessageType MessageType,
    HashSet<uint> SeenByUsers,
    TimeSpan SentAt,
    string Content);

public enum NanoChatMessageFlags : byte
{
    None = 0,
    Blocked = 1,
    FailedToSend = 2,
}

public enum NanoChatConversationFlags : byte
{
    None = 0,
    Silenced = 1
}

[ByRefEvent]
public record struct NanoChatAttemptContactEvent(HashSet<uint> Targets, bool Cancelled = false);

public sealed class NanoChatNewConversationEvent(NanoChatConversation conversation) : EntityEventArgs
{
    public NanoChatConversation Conversation { get; } = conversation;
}

[ByRefEvent]
public record struct NanoChatUpdatedConversationEvent(NanoChatConversation Conversation);

[ByRefEvent]
public record struct NanoChatUsersUpdatedEvent(Dictionary<uint, NanoChatUser> RelevantUsers);

[ByRefEvent]
public record struct NanoChatNewMessageEvent(NanoChatMessage Message);

[ByRefEvent]
public record struct NanoChatMessageChangedEvent(NanoChatMessage NewMessage);

[ByRefEvent]
public record struct NanoChatDeleteMessageEvent(Guid MessageId);
