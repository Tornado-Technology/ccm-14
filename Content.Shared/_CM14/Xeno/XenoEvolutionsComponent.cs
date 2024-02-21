using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._CM14.Xeno;

[RegisterComponent]
public sealed partial class XenoEvolutionsComponent : Component
{
    public const float BaseEvolutionModifer = 1f;

    [DataField("action", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoEvolution";

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
    public string Prototype = "MobFaceHuggerXeno";

    [DataField("evolution"), ViewVariables(VVAccess.ReadWrite)]
    public float Evolution = 0f;
}
