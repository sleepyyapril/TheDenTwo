using Content.Server.CartridgeLoader;
using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Content.Shared.CartridgeLoader;
using Content.Shared.PDA;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem : EntitySystem
{
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoaderSystem = null!;

    private Dictionary<uint, NanoChatUser> _users = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanoChatCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
        SubscribeLocalEvent<NanoChatCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
        SubscribeLocalEvent<NanoChatCartridgeComponent, EntParentChangedMessage>(OnCartridgeParentChanged);
    }

    private void OnCartridgeParentChanged(Entity<NanoChatCartridgeComponent> ent, ref EntParentChangedMessage args)
    {

    }

    private void OnUiReady(Entity<NanoChatCartridgeComponent> ent, ref CartridgeUiReadyEvent args)
    {
        if (!TryGetNanoChatCard(ent, out var card))
            return;

        UpdateUiState(ent, args.Loader);
    }

    private void OnUiMessage(Entity<NanoChatCartridgeComponent> ent, ref CartridgeMessageEvent args)
    {
        if (args is not NanoChatUiMessageEvent message)
            return;

        if (!TryGetNanoChatCard(ent, out var card))
            return;

        switch (message.Payload)
        {
            case NanoChatUiConversationCreatedEvent ev:
                OnConversationCreated(card.Value, ev);
                break;
            case NanoChatUiCheckedConversationEvent ev:
                OnCheckedConversation(card.Value, ev);
                break;
            case NanoChatUiMessageReceivedEvent ev:
                OnMessageReceived(card.Value, ev);
                break;
            case NanoChatUiMessageEditedEvent ev:
                OnMessageEdited(card.Value, ev);
                break;
            case NanoChatUiMessageDeletedEvent ev:
                OnMessageDeleted(card.Value, ev);
                break;
        }
    }

    private void UpdateUiState(Entity<NanoChatCardComponent> ent, EntityUid loader)
    {
        var state = new NanoChatUiState
        {
            Conversations = ent.Comp.Conversations,
            Messages = ent.Comp.Messages,
            Users = ent.Comp.Users,
            SilencedConversations = ent.Comp.SilencedConversations,
            UnreadMessages = ent.Comp.UnreadMessages
        };

        _cartridgeLoaderSystem.UpdateCartridgeUiState(loader, state);
    }
}
