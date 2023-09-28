using Content.Shared.Actions;

namespace Content.Shared.Alien;

public abstract class SharedBrainHuggingSystem : EntitySystem
{
    [Dependency] protected readonly SharedAudioSystem _audioSystem = default!;
}

public sealed partial class BrainSlugJumpActionEvent : WorldTargetActionEvent { }
