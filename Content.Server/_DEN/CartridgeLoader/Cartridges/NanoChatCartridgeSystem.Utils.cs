using System.Diagnostics.CodeAnalysis;
using Content.Shared._DEN.CartridgeLoader.Cartridges;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem
{
    public bool CanMessageUser(Entity<NanoChatCardComponent> ent, uint target)
    {
        return true;
    }

    public bool TryGetNanoChatCard(Entity<NanoChatCartridgeComponent> ent,
        [NotNullWhen(true)] out Entity<NanoChatCardComponent>? card)
    {
        card = null;

        if (ent.Comp.Card == null || !Exists(ent.Comp.Card)
            || !TryComp<NanoChatCardComponent>(ent.Comp.Card, out var cardComp))
            return false;

        card = (ent.Comp.Card.Value, cardComp);
        return true;
    }

    private bool TryUpdateCard(Entity<NanoChatCartridgeComponent> ent,
        [NotNullWhen(true)] out Entity<NanoChatCardComponent>? resultCard)
    {
        if (!TryGetNanoChatCard(ent, out var maybeCard)
            || maybeCard is not { } card)
        {
            ent.Comp.Card = null;
            resultCard = null;
            return false;
        }

        ent.Comp.Card = card;
        resultCard = card;
        return true;
    }
}
