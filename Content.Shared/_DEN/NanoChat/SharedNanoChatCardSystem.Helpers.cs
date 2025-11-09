using System.Diagnostics.CodeAnalysis;
using Content.Shared._DEN.CartridgeLoader.Cartridges;

namespace Content.Shared._DEN.NanoChat;

public abstract partial class SharedNanoChatCardSystem
{
    private uint? GetNumber(Entity<NanoChatCardComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return null;

        return ent.Comp.PersonalNumber;
    }

    public bool TryGetNumber(Entity<NanoChatCardComponent?> ent,
        [NotNullWhen(true)] out uint? number)
    {
        number = GetNumber(ent);
        return number != null;
    }

    public void SetNumber(Entity<NanoChatCardComponent?> ent, uint number)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        ent.Comp.PersonalNumber = number;
        Dirty(ent);
    }

    private bool TryGetNanoChatLoader(Entity<NanoChatCardComponent?> ent,
        out EntityUid? loaderUid)
    {
        if (!Resolve(ent, ref ent.Comp)
            || ent.Comp.LoaderUid == null || !Exists(ent.Comp.LoaderUid))
        {
            loaderUid = null;
            return false;
        }

        loaderUid = ent.Comp.LoaderUid;
        return true;
    }
}
