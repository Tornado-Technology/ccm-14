using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoScreechComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoScreech";

    [DataField("radius")]
    public float Radius = 5f;

    [DataField("refresh")]
    public bool Refresh = true;

    /// <summary>
    /// Effect duration in seconds;
    /// </summary>
    [DataField("stunTime")]
    public float StunTime = 2f;
}
