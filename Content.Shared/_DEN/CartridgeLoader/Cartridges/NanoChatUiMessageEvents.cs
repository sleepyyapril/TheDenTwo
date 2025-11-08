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
/// An event called by group chats and when someone new sends you a message.
/// </summary>
/// <param name="conversationId">The conversation ID.</param>
/// <param name="members">The members in the conversation.</param>
[Serializable, NetSerializable]
public sealed class NanoChatUiConversationCreatedEvent(
    Guid conversationId,
    HashSet<uint> members) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public Guid ConversationId = conversationId;

    [ViewVariables]
    public HashSet<uint> Members = members;
}

[Serializable, NetSerializable]
public sealed class NanoChatUiMessageReceivedEvent(NanoChatMessage message) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public NanoChatMessage Message = message;
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
public sealed class NanoChatUiCheckedConversationEvent(
    Guid conversationId,
    uint seenById) : INanoChatUiMessageEvent
{
    [ViewVariables]
    public Guid ConversationId = conversationId;

    [ViewVariables]
    public uint SeenById = seenById;
}
