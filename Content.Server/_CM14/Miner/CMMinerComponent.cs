namespace Content.Server._CM14.Miner;

[RegisterComponent]
public sealed partial class CMMinerComponent : Component
{
    [DataField]
    public int BalanceGeneration = 80;
}
