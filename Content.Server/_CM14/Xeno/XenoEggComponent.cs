using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno;

[RegisterComponent]
public sealed partial class XenoEggComponent : Component
{
    [DataField("hatchingTime")]
    public TimeSpan HatchingTime = TimeSpan.FromSeconds(60f);

    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan CurHatchingTime = TimeSpan.FromSeconds(0f);

    [DataField("target", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Target = "MobFaceHuggerXeno";
}
