using Content.Shared._DEN.CartridgeLoader.Cartridges;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem
{
    private void OnConversationCreated(
        Entity<NanoChatCardComponent> cartridge,
        NanoChatUiConversationCreatedEvent ev)
    {

    }

    private void OnCheckedConversation(
        Entity<NanoChatCardComponent> cartridge,
        NanoChatUiCheckedConversationEvent ev)
    {

    }

    private void OnMessageReceived(Entity<NanoChatCardComponent> cartridge,
        NanoChatUiMessageReceivedEvent ev)
    {

    }

    private void OnMessageEdited(Entity<NanoChatCardComponent> cartridge,
        NanoChatUiMessageEditedEvent ev)
    {

    }

    private void OnMessageDeleted(Entity<NanoChatCardComponent> cartridge,
        NanoChatUiMessageDeletedEvent ev)
    {

    }
}
