using Content.Server._CM14.Rules.Barrier.Components;
using Content.Server.Chat.Systems;
using Robust.Shared.Spawners;
using Content.Server.Audio;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.GameTicking.Components;


namespace Content.Server._CM14.Rules.Barrier;

public sealed class CMBarrierRule : GameRuleSystem<CMBarrierRuleComponent>
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;

    [Dependency] private readonly ServerGlobalSoundSystem _sound = default!;

    protected override void ActiveTick(EntityUid uid, CMBarrierRuleComponent component, GameRuleComponent gameRule,
        float frameTime)
    {
        if (component.BarrierTimer - frameTime < 0 )
        {
            GameTicker.EndGameRule(uid, gameRule);
            return;
        }

        component.BarrierTimer -= frameTime;
    }

    private void BarrierDisable()
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

    protected override void Ended(EntityUid uid, CMBarrierRuleComponent cmBarrierRuleComponent, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, cmBarrierRuleComponent, gameRule, args);
        BarrierDisable();
    }
}
