using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoSpitComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionDefilerDefaultSpit";

    [DataField("projectile", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Projectile = "ProjectileDefilerAcidSpit";

    [DataField("speed")]
    public float Speed = 10f;
}

/// <summary>
///     A dull crutch that allows you to cram several identical actions into one system.
///     This MUST be remade in the future.
/// </summary>
[RegisterComponent]
public sealed partial class XenoSpit2Component : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionDefilerDefaultSpit";

    [DataField("projectile", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Projectile = "ProjectileDefilerAcidSpit";

    [DataField("speed")]
    public float Speed = 10f;
}

[RegisterComponent]
public sealed partial class XenoSpitRejuvenateComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoRejuvenate";

    [DataField("projectile", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Projectile = "ProjectileXenoRejuvenate";

    [DataField("speed")]
    public float Speed = 25f;
}
