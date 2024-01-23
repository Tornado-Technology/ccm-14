using Content.Server.GameTicking.Rules.Components;
using Content.Server.GameTicking.Rules;
using Content.Server.RoundEnd;
using Content.Server.GameTicking;
using Content.Server.Nuke;
using Content.Server.Chat.Managers;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs;
using Content.Shared.Destructible;
using Content.Server.Xeno.Components;
using WinType = Content.Server.Xeno.Components.WinType;
using WinCondition = Content.Server.Xeno.Components.WinCondition;
using Content.Server.Chat.Systems;
using Robust.Shared.Player;
using Content.Shared.Xeno;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Xeno.Systems;

public sealed class XenoRuleSystem : GameRuleSystem<XenoRuleComponent>
{
    public int Eggs { get; private set; }
    public int Marines { get; private set; }
    public int Xenos { get; private set; }

    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;


    private float _announcmentTime = 0f;
    public const float AnnouncmentTime = 300f; // 5 min;

    private EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent> Query =>
        EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent>();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartAttemptEvent>(OnStartAttempt);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndText);
        SubscribeLocalEvent<NukeExplodedEvent>(OnNukeExploded);

        SubscribeLocalEvent<XenoComponent, MobStateChangedEvent>(OnXenoMobStateChanged);
        SubscribeLocalEvent<MarineComponent, MobStateChangedEvent>(OnMarineMobStateChanged);

        SubscribeLocalEvent<XenoEggComponent, DestructionEventArgs>(OnDestruction);
        SubscribeLocalEvent<XenoEggComponent, ComponentStartup>(OnEggComponentInit);
        SubscribeLocalEvent<NukeDisarmSuccessEvent>(OnNukeDisarm);
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
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            CheckRoundShouldEnd();
            _chatSystem.DispatchGlobalAnnouncement(
                Loc.GetString("ai-announcement-warning", ("xenos", Xenos), ("marines", Marines)),
                Loc.GetString("ai-announcement-sender"));
        }
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        return;

        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            var minPlayers = xeno.MinPlayers;
            if (!ev.Forced && ev.Players.Length < minPlayers)
            {
                _chatManager.SendAdminAnnouncement(Loc.GetString("xeno-not-enough-ready-players",
                    ("readyPlayersCount", ev.Players.Length), ("minimumPlayers", minPlayers)));
                ev.Cancel();
                continue;
            }

            if (ev.Players.Length != 0)
                continue;

            _chatManager.DispatchServerAnnouncement(Loc.GetString("xeno-no-one-ready"));
            ev.Cancel();
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

        foreach (var (tagComp, mobStateComp) in EntityQuery<T, MobStateComponent>())
        {
            if (mobStateComp.CurrentState != MobState.Alive)
                continue;

            count++;
        }

        return count;
    }
}
