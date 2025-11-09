using System.Linq;
using Content.Shared._DEN.CartridgeLoader.Cartridges;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem
{
    private void OnAttemptConversationCreated(
        Entity<NanoChatCardComponent> ent,
        NanoChatUiConversationCreatedEvent ev)
    {
        var attemptEv = new NanoChatAttemptContactEvent(ev.Members);
        RaiseLocalEvent(ref attemptEv);

        if (attemptEv.Cancelled)
            return;

        var conversationId = Guid.NewGuid();
        var conversation = new NanoChatConversation(conversationId,
                attemptEv.Targets,
                [],
                NanoChatConversationFlags.None);

        var successEvent = new NanoChatNewConversationEvent(conversation);
        RaiseLocalEvent(successEvent);
    }

    private void OnSetCurrentConversation(
        Entity<NanoChatCardComponent> ent,
        NanoChatUiSetCurrentConversationEvent ev)
    {
        if (ev.ConversationId == null)
        {
            ent.Comp.CurrentChat = null;
            UpdateUiState(ent);
            return;
        }

        if (!ent.Comp.Conversations.TryGetValue(ev.ConversationId.Value, out _))
            return;

        ent.Comp.CurrentChat = ev.ConversationId;
        UpdateUiState(ent);
    }

    private void OnAttemptMessageSent(Entity<NanoChatCardComponent> ent,
        NanoChatUiMessageSentEvent ev)
    {

    }

    private void OnMessageEdited(Entity<NanoChatCardComponent> ent,
        NanoChatUiMessageEditedEvent ev)
    {

    }

    private void OnMessageDeleted(Entity<NanoChatCardComponent> ent,
        NanoChatUiMessageDeletedEvent ev)
    {

    }
}
