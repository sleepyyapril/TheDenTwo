using Robust.Shared.Serialization;

namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NanoChatCardComponent : Component
{
    [ViewVariables]
    public Dictionary<Guid, NanoChatConversation> Conversations { get; private set; } = new();

    [ViewVariables]
    public Dictionary<int, NanoChatUser> Users { get; private set; } = new();

    [ViewVariables]
    public Dictionary<Guid, NanoChatMessage> Messages { get; private set; } = new();

    [ViewVariables]
    public HashSet<Guid> SilencedConversations { get; set; } = new();

    [ViewVariables]
    public HashSet<Guid> UnreadMessages { get; set; } = new();
}

[Serializable, NetSerializable]
public record struct NanoChatUser(int UniqueId, string Name, string Job);

[Serializable, NetSerializable]
public record struct NanoChatConversation(
    HashSet<int> Members,
    HashSet<Guid> Messages);

[Serializable, NetSerializable]
public record struct NanoChatMessage(
    NanoChatUser Sender,
    Guid ConversationId,
    NanoChatMessageType MessageType,
    HashSet<int> SeenByUsers,
    TimeSpan SentAt,
    string Content);

[ByRefEvent]
public record struct NanoChatNewConversationEvent(Guid ConversationId);

[ByRefEvent]
public record struct NanoChatNewMessageEvent(NanoChatMessage Message);

[ByRefEvent]
public record struct NanoChatEditMessageEvent(NanoChatMessage Message);

[ByRefEvent]
public record struct NanoChatDeleteMessageEvent(NanoChatMessage Message);
