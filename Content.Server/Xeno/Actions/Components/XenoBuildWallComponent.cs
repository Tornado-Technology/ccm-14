using Robust.Shared.Prototypes;

namespace Content.Server.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoBuildWallComponent : Component
{
    [DataField]
    public EntProtoId Action = "ActionXenoBuildWall";

    [DataField]
    public EntProtoId WallPrototype = "XenoWallFragile";

    [DataField]
    public float TimeUsage = 10f;
}
