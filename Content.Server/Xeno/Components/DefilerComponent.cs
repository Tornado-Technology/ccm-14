using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Server.Xeno.Components
{
    [RegisterComponent]
    public sealed partial class DefilerComponent : Component
    {
        [DataField("defilerSpitAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DefilerSpit = "ActionDefilerSpit";

        [DataField("defilerExplosiveAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DefilerExplosive = "ActionDefilerExplosive";
    }
}
