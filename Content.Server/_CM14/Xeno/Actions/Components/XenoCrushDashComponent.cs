using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoCrushDashComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoCrushDash";

    [DataField("dashForce")]
    public float DashForce = 35f;

    /// <summary>
    /// Effect duration in seconds;
    /// </summary>
    [DataField("stunTime")]
    public float StunTime = 4f;
}
