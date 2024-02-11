using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoEndureComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoEndure";

    [DataField("passiveModifierSet", customTypeSerializer: typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
    public string PassiveModifierSet = "XenoRavager";

    [DataField("activeModifierSet", customTypeSerializer: typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
    public string ActiveModifierSet = "XenoRavagerCrest";

    [DataField("enabled")]
    public bool Enabled;
}
