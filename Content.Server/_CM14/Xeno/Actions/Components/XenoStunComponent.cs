using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoStunComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoStun";

    [DataField("refresh")]
    public bool Refresh = true;

    /// <summary>
    ///     Effect duration in seconds;
    /// </summary>
    [DataField("stunTime")]
    public float StunTime = 4f;

    [DataField]
    public float DistanceTolerance = 1f;
}
