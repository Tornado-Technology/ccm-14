using Robust.Shared.Prototypes;

namespace Content.Shared.Xeno;

[RegisterComponent]
public sealed partial class XenoComponent : Component
{
    // [DataField]
    // public EntProtoId ActionNightVision = "ActionXenoNightVision";

    [DataField("researchPoints"), ViewVariables(VVAccess.ReadWrite)]
    public int ResearchPoints = 1000;
}
