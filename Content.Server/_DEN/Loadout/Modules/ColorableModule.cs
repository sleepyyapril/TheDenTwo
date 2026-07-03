using Content.Shared._DEN.Recolor;
using Robust.Shared.Prototypes;

namespace Content.Server._DEN.Loadout.Modules;

[Serializable]
public sealed class ColorableModule : ILoadoutModule
{
    /// <summary>
    /// The hex color to tint the item.
    /// </summary>
    public required string ColorTint {  get; set; }

    public void OnEntitySpawn(IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityUid uid)
    {
        var recolorSystem = entMan.System<RecolorSystem>();
        var hexColor = Color.TryFromHex(ColorTint);

        if (hexColor is null)
            return;

        recolorSystem.Recolor(uid, hexColor.Value, false);
    }
}
