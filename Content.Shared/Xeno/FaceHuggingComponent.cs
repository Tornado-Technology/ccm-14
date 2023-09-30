using Content.Shared.Actions;
using Content.Shared.Alien;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;
namespace Content.Server.Alien;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedFaceHuggingSystem))]
public sealed partial class FaceHuggingComponent : Component
{

    [DataField("chansePounce"), ViewVariables(VVAccess.ReadWrite)]
    public static int ChansePounce = 33;


    [ViewVariables(VVAccess.ReadWrite), DataField("soundFaceHuggerJump")]
    public SoundSpecifier? SoundFaceHuggerJump = new SoundPathSpecifier("/Audio/Animals/facehugger_scream.ogg");


    [DataField("faceHuggerJumpAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FaceHuggerJumpAction = "ActionFaceHuggerJump";


    //[DataField("actionFaceHuggerJump", required: true)]
    public sealed partial class FaceHuggerJumpActionEvent : WorldTargetActionEvent { }


    [ViewVariables(VVAccess.ReadWrite), DataField("soundFaceHugging")]
    public SoundSpecifier? SoundFaceHugging = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f),
    };
}
