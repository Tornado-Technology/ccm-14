using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Xeno.Components;

[RegisterComponent]
public sealed partial class XenoLayEggComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoLayEgg";

    [DataField("eggPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string EggPrototype = "XenoEgg";
}
