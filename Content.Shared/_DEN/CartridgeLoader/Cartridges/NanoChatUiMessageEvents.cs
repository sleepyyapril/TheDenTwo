using Content.Shared.CartridgeLoader;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

public interface INanoChatUiMessageEvent;

[Serializable, NetSerializable]
public sealed class NanoChatUiMessageEvent(INanoChatUiMessageEvent payload) : CartridgeMessageEvent
{
    public readonly INanoChatUiMessageEvent Payload = payload;
}

/// <summary>
/// An event called by trying to create a group chat or message someone new.
/// </summary>
/// <param name="members">The members in the conversation.</param>
[Serializable, NetSerializable]
public sealed class NanoChatUiConversationCreatedEvent(HashSet<uint> members) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public HashSet<uint> Members = members;
}

[Serializable, NetSerializable]
public sealed class NanoChatUiMessageSentEvent(
    NanoChatMessageType messageType,
    TimeSpan sentAt,
    string content) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public NanoChatMessageType MessageType = messageType;

    [ViewVariables]
    public TimeSpan SentAt = sentAt;

    [ViewVariables]
    public string Content = content;
}

[Serializable, NetSerializable]
public sealed class NanoChatUiMessageEditedEvent(
    Guid messageId,
    string newContent) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public Guid MessageId = messageId;

    [ViewVariables]
    public string NewContent = newContent;
}

[Serializable, NetSerializable]
public sealed class NanoChatUiMessageDeletedEvent(
    Guid messageId) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public Guid MessageId = messageId;
}

[Serializable, NetSerializable]
public sealed class NanoChatUiSetCurrentConversationEvent(
    Guid? conversationId) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public Guid? ConversationId = conversationId;
}
