using Content.Server.GameTicking;
using Content.Server.Chat.Systems;
using Robust.Shared.Spawners;
using Content.Server.Audio;


namespace Content.Server._CM14.Barrier;

public sealed class CMBarrierSystem : EntitySystem
{
    [Dependency] private readonly GameTicker _ticker = default!;

    [Dependency] private readonly ChatSystem _chatSystem = default!;

    [Dependency] private readonly ServerGlobalSoundSystem _sound = default!;

    public float BarrierTimer = 1200f;
    private const float BarrierTimerConst = 1200f;

    public bool BarrierCountdown = true;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoundStartAttemptEvent>(OnStartAttempt);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_ticker.RunLevel != GameRunLevel.InRound || !BarrierCountdown)
            return;
        if (BarrierTimer - frameTime < 0 && BarrierCountdown)
        {
            BarrierDisable();
            BarrierCountdown = false;
            return;
        }

        BarrierTimer -= frameTime;
    }

    public void BarrierDisable()
    {
        _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ai-announcement-fob"),
            Loc.GetString("ai-announcement-sender"));
        var first = true;
        var query = EntityQueryEnumerator<CMBarrierComponent>();
        while (query.MoveNext(out var uid, out var barrier))
        {
            EnsureComp<TimedDespawnComponent>(uid);
            if (!first)
                continue;
            _sound.PlayGlobalOnStation(uid, "/Audio/_CM/Misc/fob_protection_sound.ogg");
            first = false;
        }
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        BarrierTimer = BarrierTimerConst;
        BarrierCountdown = true;
    }

}
