using Content.Shared._CD.Traits; // TheDen - Move to shared
using Content.Shared.Body.Systems; // TheDen - Move to shared
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chat.TypingIndicator; // TheDen - Refactor to be partial

public abstract partial class SharedTypingIndicatorSystem // TheDen - Refactor to be partial
{
    private static readonly ProtoId<TypingIndicatorPrototype> RobotTypingIndicator = "robot"; // Misfit - Type safety
    private static readonly ProtoId<ReagentPrototype> SynthBlood = "SynthBlood"; // Misfit - Type safety

    [Dependency] private readonly SharedBloodstreamSystem _bloodstream = default!; // TheDen - Move to shared

    public void InitializeCD() // TheDen - Refactor to be partial
    {
        SubscribeLocalEvent<SynthComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, SynthComponent component, ComponentStartup args)
    {
        SetIndicatorPrototype(uid, RobotTypingIndicator); // TheDen - Switch to set indicator prototype

        // Give them synth blood. Ion storm notif is handled in that system
        _bloodstream.ChangeBloodReagent(uid, SynthBlood); // Misfit - Type safety
    }
}
