namespace Content.Server._CM14.Xeno;

[RegisterComponent]
public sealed partial class GravityMovementSlowComponent : Component
{
    public bool IsEnable = false;

    [DataField]
    public float SlowPercentage = 0.65f;
}
