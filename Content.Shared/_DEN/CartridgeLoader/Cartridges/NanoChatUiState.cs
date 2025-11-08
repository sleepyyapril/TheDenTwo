namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

public sealed class NanoChatUiState : BoundUserInterfaceState
{
    public Dictionary<Guid, NanoChatConversation> Conversations = new();
    public Dictionary<int, NanoChatUser> Users = new();
    public Dictionary<Guid, NanoChatMessage> Messages = new();
    public HashSet<Guid> SilencedConversations = new();
    public HashSet<Guid> UnreadMessages = new();
}
