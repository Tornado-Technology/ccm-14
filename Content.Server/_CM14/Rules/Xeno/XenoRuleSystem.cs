using Content.Server._CM14.Mapping;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Nuke;
using Content.Server.RoundEnd;
using Content.Shared.Destructible;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Content.Server._CM14.Xeno;
using Content.Server.Communications;
using Content.Shared._CM14.Xeno;
using Content.Shared.Mobs.Systems;
using Content.Shared.Overlays;

namespace Content.Server._CM14.Rules.Xeno;

public sealed class XenoRuleSystem : GameRuleSystem<XenoRuleComponent>
{
    private int Eggs { get; set; }
    private int Marines { get; set; }
    private int Xenos { get; set; }

    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;


    private float _announcmentTime = 0f;
    public const float AnnouncmentTime = 300f; // 5 min;

    private EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent> Query =>
        EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent>();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndText);
        SubscribeLocalEvent<NukeExplodedEvent>(OnNukeExploded);
        SubscribeLocalEvent<NukeArmSuccessEvent>(OnNukeArmed);

        SubscribeLocalEvent<XenoComponent, MobStateChangedEvent>(OnXenoMobStateChanged);
        SubscribeLocalEvent<MarineComponent, MobStateChangedEvent>(OnMarineMobStateChanged);

        SubscribeLocalEvent<XenoEggComponent, DestructionEventArgs>(OnDestruction);
        SubscribeLocalEvent<XenoEggComponent, ComponentStartup>(OnEggComponentInit);
        SubscribeLocalEvent<NukeDisarmSuccessEvent>(OnNukeDisarm);

        SubscribeLocalEvent<CommunicationConsoleCallShuttleAttemptEvent>(OnShuttleCallAttempt);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_ticker.RunLevel != GameRunLevel.InRound)
            return;

        _announcmentTime += frameTime;

        if (_announcmentTime < AnnouncmentTime)
            return;

        _announcmentTime -= AnnouncmentTime;

        var query = Query;
        while (query.MoveNext(out var uid, out _, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            CheckRoundShouldEnd();
            _chatSystem.DispatchGlobalAnnouncement(
                Loc.GetString("ai-announcement-warning", ("xenos", Xenos), ("marines", Marines)),
                Loc.GetString("ai-announcement-sender"));
        }
    }

    private void OnEggComponentInit(EntityUid uid, XenoEggComponent component, ComponentStartup args)
    {
        CheckRoundShouldEnd();
    }

    private void OnDestruction(EntityUid uid, XenoEggComponent component, DestructionEventArgs args)
    {
        CheckRoundShouldEnd();
    }

    private void OnNukeDisarm(NukeDisarmSuccessEvent ev)
    {
        CheckRoundShouldEnd();
    }

    private void OnMarineMobStateChanged(EntityUid uid, MarineComponent component, MobStateChangedEvent ev)
    {
        if (ev.NewMobState != MobState.Dead)
            return;

        CheckRoundShouldEnd();
    }

    private void OnXenoMobStateChanged(EntityUid uid, XenoComponent component, MobStateChangedEvent ev)
    {
        if (ev.NewMobState != MobState.Dead)
            return;

        CheckRoundShouldEnd();
    }

    private void CheckRoundShouldEnd()
    {
        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            // We had tick update, we dont want to collect all condition states from last update
            xeno.WinConditions.RemoveAll((WinCondition type) =>
                type == WinCondition.AllMarineDied ||
                type == WinCondition.EggsWinningCount ||
                type == WinCondition.AllXenoDied ||
                type == WinCondition.RoyalQueenExist);

            Eggs = Count(typeof(XenoEggComponent));
            Xenos = GetMobAliveCount<XenoComponent>();
            Marines = GetMobAliveCount<MarineComponent>();

            var sender = Loc.GetString("ai-announcement-sender");

            if (Eggs == 20)
            {
                _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ai-announcement-xeno-egg-count-warning-medium"),
                    sender, false, colorOverride: Color.Yellow);
                _audio.PlayGlobal("/Audio/Misc/redalert.ogg", Filter.Broadcast(), recordReplay: true);
            }

            if (Eggs == 40)
            {
                _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ai-announcement-xeno-egg-count-warning-hight"),
                    sender, false, colorOverride: Color.Orange);
                _audio.PlayGlobal("/Audio/Misc/siren.ogg", Filter.Broadcast(), recordReplay: true);
            }

            if (Eggs >= xeno.WinningXenoEggCount)
            {
                _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ai-announcement-xeno-egg-count-warning-crit"),
                    sender, false, colorOverride: Color.Red);
                _audio.PlayGlobal("/Audio/Misc/delta.ogg", Filter.Broadcast(), recordReplay: true);

                xeno.WinConditions.Add(WinCondition.EggsWinningCount);
                SetWinType(uid, WinType.XenoMinor, xeno);
                continue;
            }

            if (Xenos == 0)
            {
                xeno.WinConditions.Add(WinCondition.AllXenoDied);
                SetWinType(uid, WinType.MarineMinor, xeno);
                continue;
            }

            if (Marines == 0)
            {
                xeno.WinConditions.Add(WinCondition.AllMarineDied);
                SetWinType(uid, WinType.XenoMajor, xeno);
            }

            // if all checks false, trying to predict game end (for force end round behavior)
            // check a queen exist
            // @TODO add xenoRoleComponent or something else to get easy way get any type of xeno
            var t5query = EntityQueryEnumerator<XenoTierComponent>();
            while (t5query.MoveNext(out var tempQueenUid, out var xenoTier))
            {
                if (xenoTier.Tier == 4 && TryComp<MetaDataComponent>(tempQueenUid, out var queenMeta))
                {
                    if (queenMeta.EntityPrototype != null)
                    {
                        if (queenMeta.EntityPrototype.ID.Equals("MobQueenXeno") && _mobStateSystem.IsDead(tempQueenUid))
                        {
                            xeno.WinType = WinType.MarineMinor;
                        }
                    }
                }
                else if (xenoTier.Tier == 5)
                {
                    if (_mobStateSystem.IsDead(tempQueenUid))
                    {
                        xeno.WinType = WinType.MarineMinor;
                        continue;
                    }
                    xeno.WinType = WinType.XenoMinor;
                    break;
                }
            }
            // end check queen hack

        }
    }

    private void OnNukeExploded(NukeExplodedEvent ev)
    {
        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            if (ev.OwningStation == null)
            {
                xeno.WinConditions.Add(WinCondition.NukeExplodedOnIncorrectLocation);
                SetWinType(uid, WinType.Neutral, xeno);
                continue;
            }

            if (ev.OwningStation == xeno.MarineOutpost)
            {
                xeno.WinConditions.Add(WinCondition.NukeExplodedOnMarineOutpost);
                SetWinType(uid, WinType.Neutral, xeno);
                continue;
            }

            if (ev.OwningStation == xeno.XenoPlanet)
            {
                xeno.WinConditions.Add(WinCondition.NukeExplodedOnXenoPlanet);
                SetWinType(uid, WinType.MarineMajor, xeno);
                continue;
            }

            xeno.WinConditions.Add(WinCondition.NukeExplodedOnIncorrectLocation);
            SetWinType(uid, WinType.Neutral, xeno);
        }
    }

    private void OnNukeArmed(NukeArmSuccessEvent ev)
    {
        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            // Start temp HACK
            // we need this vars only for nukes, hack is event oriented
            if (xeno.MarineOutpost == null)
            {
                var tempQuery = EntityQueryEnumerator<MarineMapComponent>();
                while (tempQuery.MoveNext(out var mapUid, out _))
                {
                    xeno.MarineOutpost = mapUid;
                }
            }

            if (xeno.XenoPlanet == null)
            {
                var tempQuery = EntityQueryEnumerator<XenoMapComponent>();
                while (tempQuery.MoveNext(out var mapUid, out _))
                {
                    xeno.XenoPlanet = mapUid;
                }
            }
            // End temp HACK

            // only predict logic, which didnt collide with other predicts
            if (ev.OwningStation == xeno.XenoPlanet)
            {
                xeno.WinType = WinType.MarineMajor;
            }
        }
    }

    private void OnRoundEndText(RoundEndTextAppendEvent ev)
    {
        foreach (var xeno in EntityQuery<XenoRuleComponent>())
        {
            var winText = Loc.GetString($"xeno-{xeno.WinType.ToString().ToLower()}");
            ev.AddLine(winText);

            foreach (var cond in xeno.WinConditions)
            {
                var text = Loc.GetString($"xeno-cond-{cond.ToString().ToLower()}");
                ev.AddLine(text);
            }
        }
    }

    private void SetWinType(EntityUid uid, WinType type, XenoRuleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.WinType = type;
        _roundEndSystem.EndRound();
    }

    private int GetMobAliveCount<T>() where T : Component
    {
        var count = 0;

        foreach (var (_, mobStateComp) in EntityQuery<T, MobStateComponent>())
        {
            if (mobStateComp.CurrentState != MobState.Alive)
                continue;

            count++;
        }

        return count;
    }

    private void OnShuttleCallAttempt(ref CommunicationConsoleCallShuttleAttemptEvent args)
    {
        args.Cancelled = false;
        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            xeno.WinConditions.Add(WinCondition.EmergencyShuttleCalled);
            // round end predict
            xeno.WinType = WinType.XenoMinor;
        }
    }
}
