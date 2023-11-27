using Content.Server.Cargo.Components;
using Content.Server.Cargo.Systems;

namespace Content.Server.Miner;

public sealed class MinerSystem : EntitySystem
{
    [Dependency] private readonly CargoSystem _cargo = default!;

    private float _updateTimer = 0.0f;
    private const float UpdateTime = 1.0f;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _updateTimer += frameTime;
        if (_updateTimer < UpdateTime)
            return;

        _updateTimer -= UpdateTime;

        foreach (var miner in EntityQuery<MinerComponent>())
        {
            foreach (var bank in EntityQuery<StationBankAccountComponent>())
            {
                _cargo.UpdateBankAccount(bank.Owner, bank, miner.BalanceGeneration);
            }
        }
    }
}
