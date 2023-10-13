using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Xeno;

[RegisterComponent]
public sealed partial class XenoEvolutionsComponent : Component
{
    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoEvolution";

    /// <summary>
    ///     Added once per tick according to the formula Evolution += (frameTime * EvolutionModifer)
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public const float BaseEvolutionModifer = 1f;

    [DataField("evolutions"), ViewVariables(VVAccess.ReadWrite)]
    public HashSet<XenoEvolution> Evolutions = new();

    [DataField("evolution"), ViewVariables(VVAccess.ReadWrite)]
    public float Evolution = 0f;

    [DataField("EvolutionModifier"), ViewVariables(VVAccess.ReadWrite)]
    public float EvolutionModifer = BaseEvolutionModifer;

    [DataField("Enabled"), ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled = true;
}

[Serializable, NetSerializable, ImplicitDataDefinitionForInheritors]
public sealed partial class XenoEvolution
{
    [DataField("prototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>)), ViewVariables(VVAccess.ReadWrite)]
    public string Prototype = string.Empty;

    [DataField("evolution"), ViewVariables(VVAccess.ReadWrite)]
    public float Evolution = 0f;
}
