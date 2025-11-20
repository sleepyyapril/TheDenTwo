using Robust.Shared.Serialization;

namespace Content.Shared._DEN.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NanoChatCardComponent : Component
{
    [ViewVariables]
    public EntityUid? LoaderUid { get; set; }

    [ViewVariables]
    public uint? PersonalNumber { get; set; }

    [DataField]
    public bool ListNumber { get; set; } = true;
}
