using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._CM14.Xeno;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedFaceHuggingSystem))]
public sealed partial class FaceHuggingComponent : Component
{
    [DataField("faceHuggerJumpAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string FaceHuggerJumpAction = "ActionFaceHuggerJump";


    [ViewVariables(VVAccess.ReadWrite), DataField("soundFaceHugging")]
    public SoundSpecifier? SoundFaceHugging = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f),
    };
}
