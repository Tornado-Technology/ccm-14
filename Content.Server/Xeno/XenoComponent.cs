namespace Content.Server.Xeno.Components;

[RegisterComponent]
public sealed partial class XenoComponent : Component
{
    [DataField("researchPoints"), ViewVariables(VVAccess.ReadWrite)]
    public int ResearchPoints = 1000;
}
