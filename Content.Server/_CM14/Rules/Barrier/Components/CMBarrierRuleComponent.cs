namespace Content.Server._CM14.Rules.Barrier.Components;

[RegisterComponent]
public sealed partial class CMBarrierRuleComponent : Component
{
    [DataField]
    public float BarrierTimer = 1500f;
}
