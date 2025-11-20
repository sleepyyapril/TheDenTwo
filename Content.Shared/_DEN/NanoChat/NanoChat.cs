using Robust.Shared.Serialization;

namespace Content.Shared._DEN.NanoChat;

[Serializable, NetSerializable]
public record struct NanoChatConversation(
    Guid Id,
    string Title,
    string Subtitle,
    NanoChatConversationFlags Flags,
    HashSet<uint> Members,
    HashSet<Guid> Messages,
    NanoChatConversationType ConversationType)
{
    public Guid Id { get; init; } = Id;
    public string Title { get; init; } = Title;
    public string Subtitle { get; init; } = Subtitle;
    public NanoChatConversationFlags Flags { get; init; } = Flags;
    public HashSet<uint> Members { get; init; } = Members;
    public HashSet<Guid> Messages { get; init; } = Messages;
}

[Serializable, NetSerializable]
public record struct NanoChatUser(uint Id, string LastRecordedName, string LastRecordedJobTitle)
{
    public uint Id { get; init; } = Id;
    public string LastRecordedName { get; init; } = LastRecordedName;
    public string LastRecordedJobTitle { get; init; } = LastRecordedJobTitle;
}

[Serializable, NetSerializable]
public record struct NanoChatMessage(
    Guid MessageId,
    uint Sender,
    Guid ConversationId,
    NanoChatMessageFlags Flags,
    NanoChatMessageType MessageType,
    HashSet<uint> SeenByUsers,
    TimeSpan SentAt,
    string Content);

public enum NanoChatConversationFlags
{
    None = 0,
    Silenced = 1 << 0
}

public enum NanoChatMessageFlags
{
    None = 0,
    FailedToSend = 1 << 0
}

public enum NanoChatUserFlags
{
    None = 0,
    Silenced = 1 << 0
}

public enum NanoChatMessageType
{
    Embed,
    Text
}

public enum NanoChatConversationType
{
    DirectMessage,
    GroupChat
}
