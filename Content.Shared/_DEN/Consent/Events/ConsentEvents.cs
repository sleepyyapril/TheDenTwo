using Content.Shared._DEN.Consent.EntitySystems;
using Content.Shared._DEN.Consent.Prototypes;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Consent.Events;

/// <summary>
/// Used to inform same-side about updated consents, for whatever reason.
/// </summary>
/// <param name="UserId">The <see cref="NetUserId"/> of the user with their toggle updated.</param>
/// <param name="Toggle">The <see cref="UserConsentToggle"/> containing the toggle id and its new value. </param>
public record struct ConsentUpdated(NetUserId UserId, ProtoId<ConsentTogglePrototype> Toggle, bool NewValue);

/// <summary>
/// Used to update consent settings on either the client or the server.
/// </summary>
public sealed class MsgUpdateConsent : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public List<ProtoId<ConsentTogglePrototype>> NotDefaultConsents = [];

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var length = buffer.ReadVariableInt32();

        NotDefaultConsents.Clear();

        for (var i = 0; i < length; i++)
        {
            var updatedConsent = (ProtoId<ConsentTogglePrototype>) buffer.ReadString();
            NotDefaultConsents.Add(updatedConsent);
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(NotDefaultConsents.Count);

        foreach (var consentId in NotDefaultConsents)
        {
            buffer.Write(consentId);
        }
    }
}
