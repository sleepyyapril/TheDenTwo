using Content.Server.CartridgeLoader;
using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Content.Shared.CartridgeLoader;
using Content.Shared.PDA;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem : EntitySystem
{
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoaderSystem = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanoChatCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
        SubscribeLocalEvent<NanoChatCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
    }

    private void OnUiReady(Entity<NanoChatCartridgeComponent> ent, ref CartridgeUiReadyEvent args)
    {
        if (!TryUpdateCard(ent, out var card))
            return;

        UpdateUiState(card.Value, args.Loader);
    }

    private void OnUiMessage(Entity<NanoChatCartridgeComponent> ent, ref CartridgeMessageEvent args)
    {
        if (args is not NanoChatUiMessageEvent message)
            return;

        if (!TryUpdateCard(ent, out var card))
            return;

        switch (message.Payload)
        {
            case NanoChatUiConversationCreatedEvent ev:
                OnAttemptConversationCreated(card.Value, ev);
                break;
            case NanoChatUiSetCurrentConversationEvent ev:
                OnSetCurrentConversation(card.Value, ev);
                break;
            case NanoChatUiMessageSentEvent ev:
                OnAttemptMessageSent(card.Value, ev);
                break;
            case NanoChatUiMessageEditedEvent ev:
                OnMessageEdited(card.Value, ev);
                break;
            case NanoChatUiMessageDeletedEvent ev:
                OnMessageDeleted(card.Value, ev);
                break;
        }
    }

    public void UpdateUiState(Entity<NanoChatCardComponent> ent)
    {
        if (ent.Comp.LoaderUid == null || !Exists(ent.Comp.LoaderUid.Value))
            return;

        UpdateUiState(ent, ent.Comp.LoaderUid.Value);
    }

    private void UpdateUiState(Entity<NanoChatCardComponent> ent, EntityUid loader)
    {
        var state = new NanoChatUiState
        {
            Conversations = ent.Comp.Conversations,
            Messages = ent.Comp.Messages,
            RelevantUsers = ent.Comp.RelevantUsers,
            UnreadMessages = ent.Comp.UnreadMessages,
        };

        _cartridgeLoaderSystem.UpdateCartridgeUiState(loader, state);
    }
}
