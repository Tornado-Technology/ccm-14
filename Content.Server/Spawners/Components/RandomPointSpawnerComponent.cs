using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Spawners.Components;

[RegisterComponent]
public sealed partial class RandomPointSpawnerComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("prototypes", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    public List<string> Prototypes { get; set; } = new();

    [ViewVariables(VVAccess.ReadWrite), DataField("groupId")]
    public int GroupId { get; set; }

    [DataField("spawned")]
    public bool Spawned { get; set; } = false;
}
