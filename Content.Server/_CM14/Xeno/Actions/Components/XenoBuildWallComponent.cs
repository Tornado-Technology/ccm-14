using Robust.Shared.Prototypes;

namespace Content.Server._CM14.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoBuildWallComponent : Component
{
    [DataField]
    public EntProtoId Action = "ActionXenoBuildWall";

    [DataField]
    public EntProtoId WallPrototype = "XenoWallFragile";

    [DataField]
    public float TimeUsage = 6.5f;
}
