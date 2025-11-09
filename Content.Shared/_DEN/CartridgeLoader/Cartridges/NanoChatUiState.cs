namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

public sealed class NanoChatUiState : BoundUserInterfaceState
{
    public uint PersonalNumber;
    public HashSet<Guid> UnreadMessages = new();

    public Dictionary<Guid, NanoChatConversation> Conversations = new();
    public Dictionary<uint, NanoChatUser> RelevantUsers = new();
    public Dictionary<Guid, NanoChatMessage> Messages = new();
}
