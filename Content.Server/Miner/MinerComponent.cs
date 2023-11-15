namespace Content.Server.Miner;

[RegisterComponent]
public sealed partial class MinerComponent : Component
{
    [DataField]
    public int BalanceGeneration = 5;
}
