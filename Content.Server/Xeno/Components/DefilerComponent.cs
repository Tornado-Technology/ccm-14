using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Server.Xeno.Components
{
    [RegisterComponent]
    public sealed partial class DefilerComponent : Component
    {

        [DataField("defilerDefaultSpitAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DefilerDefaultSpit = "ActionDefilerDefaultSpit";

        [DataField("defilerAcidSpitAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DefilerAcidSpit = "ActionDefilerAcidSpit";

        [DataField("defilerExplosiveAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DefilerExplosive = "ActionDefilerExplosive";
    }
}
