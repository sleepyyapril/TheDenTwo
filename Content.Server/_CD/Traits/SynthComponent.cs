using Content.Shared.Chat.TypingIndicator; // TheDen - Refactor to be partial

namespace Content.Server._CD.Traits;

/// <summary>
/// Set players' blood to coolant
/// </summary>
[RegisterComponent, Access(typeof(SharedTypingIndicatorSystem))] // TheDen - Refactor to be partial
public sealed partial class SynthComponent : Component { } // Misfit - Refactor notification to own component
