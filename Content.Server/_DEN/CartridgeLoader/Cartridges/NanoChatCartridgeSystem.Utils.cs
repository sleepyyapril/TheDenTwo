using System.Diagnostics.CodeAnalysis;
using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Content.Shared.CartridgeLoader;

namespace Content.Server._DEN.CartridgeLoader.Cartridges;

public sealed partial class NanoChatCartridgeSystem
{
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

    private void UpdateCard(Entity<NanoChatCartridgeComponent> ent)
    {

    }
}
