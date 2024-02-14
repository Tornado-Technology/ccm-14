using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoVinesComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoVines";

    [DataField("vinesPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string VinesPrototype = "XenoWeeds";
}
