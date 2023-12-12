using Content.Shared.Actions;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.Alien;

public abstract class SharedFaceHuggingSystem : EntitySystem
{
    [Dependency] protected readonly SharedAudioSystem _audioSystem = default!;
}

public sealed partial class FaceHuggerJumpActionEvent : WorldTargetActionEvent { }
