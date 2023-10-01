using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Server.Xeno.Components
{
    [RegisterComponent]
    public sealed partial class SentinelComponent : Component
    {
        [DataField("sentinelDefaultSpitAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string SentinelDefaultSpit = "ActionSentinelDefaultSpit";
    }
}
