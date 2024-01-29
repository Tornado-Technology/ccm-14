using Content.Server.Cargo.Components;
using Content.Server.Cargo.Systems;

namespace Content.Server._CM14.Miner;

public sealed class CMMinerSystem : EntitySystem
{
    [Dependency] private readonly CargoSystem _cargo = default!;

    private float _updateTimer = UpdateTimeConst;
    private const float UpdateTimeConst = 1.0f;

    public override void Initialize()
    {
        base.Initialize();
        _updateTimer = UpdateTimeConst;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_updateTimer - frameTime <= 0)
        {
            _updateTimer = UpdateTimeConst;
        }
        else
        {
            _updateTimer -= frameTime;
            return;
        }

        foreach (var miner in EntityQuery<CMMinerComponent>())
        {
            var query = EntityQueryEnumerator<StationBankAccountComponent>();
            while (query.MoveNext(out var uid, out var bank))
            {
                _cargo.UpdateBankAccount(uid, bank, miner.BalanceGeneration);
            }
        }
    }
}
