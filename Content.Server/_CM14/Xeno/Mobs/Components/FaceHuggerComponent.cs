namespace Content.Server._CM14.Xeno.Mobs.Components;

[RegisterComponent]
public sealed partial class FaceHuggerComponent : Component
{

    public bool IsDeath = false;
    public EntityUid Equipped;

    [DataField("paralyzeTime"), ViewVariables(VVAccess.ReadWrite)]
    public float ParalyzeTime = 2f;
}
