using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Xeno.Components
{
    [RegisterComponent]

    public sealed partial class XenoStunComponent : Component
    {
        [DataField("xenoStunAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string XenoStun = "ActionXenoStun";
    }
}
