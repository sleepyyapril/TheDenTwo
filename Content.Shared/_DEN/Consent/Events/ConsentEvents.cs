using Content.Shared._DEN.Consent.EntitySystems;
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
public record struct ConsentUpdated(NetUserId UserId, UserConsentToggle Toggle);

/// <summary>
/// Used to update consent settings on either the client or the server.
/// </summary>
public sealed class MsgUpdateConsent : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public List<UserConsentToggle> UpdatedConsents = [];

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var length = buffer.ReadVariableInt32();

        UpdatedConsents.Clear();

        for (var i = 0; i < length; i++)
        {
            var updatedConsent = (ProtoId<ConsentTogglePrototype>) buffer.ReadString();
            var newValue = buffer.ReadBoolean();
            var newToggle = new UserConsentToggle(updatedConsent, newValue);
            UpdatedConsents.Add(newToggle);
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(UpdatedConsents.Count);

        foreach (var consentPair in UpdatedConsents)
        {
            buffer.Write(consentPair.ToggleId);
            buffer.Write(consentPair.ToggleValue);
        }
    }
}
