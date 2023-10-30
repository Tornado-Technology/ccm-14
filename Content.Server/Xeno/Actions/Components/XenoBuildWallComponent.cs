using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoBuildWallComponent : Component
{
    [DataField("actionBuild", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Action = "ActionXenoBuildWall";

    [DataField("wallPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string WallPrototype = "XenoWallFragile";
}
