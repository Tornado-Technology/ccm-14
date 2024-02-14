using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoDegradationComponent : Component
{
    [DataField("prototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>)),
     ViewVariables(VVAccess.ReadWrite)]
    public string Prototype = "MobFaceHuggerXeno";

    [DataField]
    public EntProtoId Action = "ActionXenoDegradation";

    [DataField("timeUsage")]
    public float TimeUsage = 15f;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("sound")]
    public SoundSpecifier DegradationSound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");
}
