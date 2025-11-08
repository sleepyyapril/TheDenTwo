using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NanoChatCartridgeComponent : Component
{
    [DataField]
    public EntityUid? Card;

    [DataField]
    public ProtoId<RadioChannelPrototype> RadioChannel = "Common";
}
