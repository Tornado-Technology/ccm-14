using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._CM14.Xeno;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoRageComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoRage";

    [DataField("passiveModifierSet", customTypeSerializer: typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
    public string PassiveModifierSet = "XenoRavager";

    [DataField("modifierSet", customTypeSerializer: typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
    public string ModifierSet = "XenoRavagerRage";

    [AutoNetworkedField]
    public bool Enabled;

    public TimeSpan TimeUsed;
}
