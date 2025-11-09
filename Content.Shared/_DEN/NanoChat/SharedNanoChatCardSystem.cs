using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Content.Shared.Examine;

namespace Content.Shared._DEN.NanoChat;

public abstract partial class SharedNanoChatCardSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanoChatCardComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<NanoChatCardComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (ent.Comp.PersonalNumber == null)
        {
            var noNumberExamineText = Loc.GetString("nanochat-card-examine-no-number");
            args.PushMarkup(noNumberExamineText);
            return;
        }

        var examineText = Loc.GetString("nanochat-card-examine-has-number", ("number", ent.Comp.PersonalNumber));
        args.PushMarkup(examineText);
    }
}
