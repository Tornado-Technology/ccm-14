using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoPsychicCureComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoPsychicCure";

    [DataField("healAmount")]
    public float HealAmount = 100f;
}
