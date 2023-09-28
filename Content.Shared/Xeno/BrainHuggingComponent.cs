using Content.Shared.Actions;
using Content.Shared.Alien;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;
namespace Content.Server.Alien;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedBrainHuggingSystem))]
public sealed partial class BrainHuggingComponent : Component
{

    [DataField("chansePounce"), ViewVariables(VVAccess.ReadWrite)]
    public static int ChansePounce = 33;


    [ViewVariables(VVAccess.ReadWrite), DataField("soundBrainSlugJump")]
    public SoundSpecifier? SoundBrainSlugJump = new SoundPathSpecifier("/Audio/Animals/brainslug_scream.ogg");


    [DataField("brainSlugJumpAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string BrainSlugJumpAction = "ActionBrainSlugJump";


    //[DataField("actionBrainSlugJump", required: true)]
    public sealed partial class BrainSlugJumpActionEvent : WorldTargetActionEvent { }


    [ViewVariables(VVAccess.ReadWrite), DataField("soundBrainHugging")]
    public SoundSpecifier? SoundBrainHugging = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f),
    };
}
