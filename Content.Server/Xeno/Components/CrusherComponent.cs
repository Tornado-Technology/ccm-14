using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Xeno.Components
{
    [RegisterComponent]

    public sealed partial class CrusherComponent : Component
    {
        [DataField("crusherJumpAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string CrusherJump = "ActionCrusherJump";

        [DataField("crusherStunAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string CrusherStun = "ActionCrusherStun";
    }
}
