using Content.Shared._DEN.CartridgeLoader.Cartridges;

namespace Content.Server._DEN.NanoChat;

public sealed partial class NanoChatCardSystem
{
    private readonly Dictionary<uint, NanoChatUser> _users = new();
    private readonly HashSet<uint> _updatedUsers = new();

    public void InitializeSync()
    {
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatUsersUpdatedEvent>(OnUsersUpdated);
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatNewConversationEvent>(OnNewConversation);
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatUpdatedConversationEvent>(OnUpdatedConversation);
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatNewMessageEvent>(OnNewMessage);
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatMessageChangedEvent>(OnMessageChanged);
        SubscribeLocalEvent<NanoChatCardComponent, NanoChatDeleteMessageEvent>(OnMessageDeleted);
    }

    private void SyncConversation(NanoChatConversation conversation)
    {
        var query = EntityQueryEnumerator<NanoChatCardComponent>();
        while (query.MoveNext(out var uid, out var card))
        {
            if (card.PersonalNumber == null || card.LoaderUid == null)
                continue;

            var sendConversation = conversation.Members.Contains(card.PersonalNumber.Value);

            if (!sendConversation)
                continue;

            card.Conversations[conversation.ConversationId] = conversation;
            _nanoChatCartridge.UpdateUiState((uid, card), card.LoaderUid.Value);
        }
    }

    private void SyncConversationFromMessage(Entity<NanoChatCardComponent> ent, NanoChatMessage message)
    {
        var hasConversation = ent.Comp.Conversations.TryGetValue(message.ConversationId, out var conversation);

        if (!hasConversation)
            return;

        SyncConversation(conversation);
    }

    private void OnNewConversation(Entity<NanoChatCardComponent> ent, ref NanoChatNewConversationEvent args)
    {
        SyncConversation(args.Conversation);
    }

    private void OnUpdatedConversation(Entity<NanoChatCardComponent> ent, ref NanoChatUpdatedConversationEvent args)
    {
        SyncConversation(args.Conversation);
    }

    private void OnNewMessage(Entity<NanoChatCardComponent> ent, ref NanoChatNewMessageEvent args)
    {
        SyncConversationFromMessage(ent, args.Message);
    }

    private void OnMessageChanged(Entity<NanoChatCardComponent> ent, ref NanoChatMessageChangedEvent args)
    {
        SyncConversationFromMessage(ent, args.NewMessage);
    }

    private void OnMessageDeleted(Entity<NanoChatCardComponent> ent, ref NanoChatDeleteMessageEvent args)
    {
        if (!ent.Comp.Messages.Remove(args.MessageId, out var message))
            return;

        SyncConversationFromMessage(ent, message);
    }

    private void OnUsersUpdated(Entity<NanoChatCardComponent> ent, ref NanoChatUsersUpdatedEvent ev)
    {
        var query = EntityQueryEnumerator<NanoChatCardComponent>();
        while (query.MoveNext(out var uid, out var card))
        {
            foreach (var nanoChatId in _updatedUsers)
            {
                if (!card.RelevantUsers.TryGetValue(nanoChatId, out _))
                    continue;

                card.RelevantUsers[nanoChatId] = ev.RelevantUsers[nanoChatId];
                Dirty(uid, card);
            }
        }
    }
}
