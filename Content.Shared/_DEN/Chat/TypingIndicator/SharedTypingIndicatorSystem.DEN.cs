using Robust.Shared.Prototypes;

namespace Content.Shared.Chat.TypingIndicator;

public abstract partial class SharedTypingIndicatorSystem
{
    public void SetIndicatorPrototype(EntityUid uid, ProtoId<TypingIndicatorPrototype> proto, TypingIndicatorComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.TypingIndicatorPrototype = proto;
        Dirty(uid, component);
    }
}
