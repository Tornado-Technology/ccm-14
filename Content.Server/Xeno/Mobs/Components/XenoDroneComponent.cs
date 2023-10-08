using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Xeno.Components;

[RegisterComponent]
public sealed partial class XenoDroneComponent : Component
{
    [DataField("actionBuild", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionBuild = "ActionXenoDroneBuild";

    [DataField("actionPsychicCure", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionPsychicCure = "ActionXenoDronePsychicCure";

    [DataField("buildPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string BuildingWall = "XenoWallFragile";
}
