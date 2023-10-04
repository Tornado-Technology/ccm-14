using Content.Server.GameTicking.Rules.Components;
using Content.Server.GameTicking.Rules;
using Content.Shared.Tag;
using System.Linq;
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

namespace Content.Server.Xeno.Systems;

public sealed class XenoRuleSystem : GameRuleSystem<XenoRuleComponent>
{
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;

    private EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent> Query => EntityQueryEnumerator<XenoRuleComponent, GameRuleComponent>();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartAttemptEvent>(OnStartAttempt);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndText);
        SubscribeLocalEvent<NukeExplodedEvent>(OnNukeExploded);
        SubscribeLocalEvent<XenoComponent, MobStateChangedEvent>(OnXenoMobStateChanged);
        SubscribeLocalEvent<MarineComponent, MobStateChangedEvent>(OnMarineMobStateChanged);
        SubscribeLocalEvent<XenoEggComponent, DestructionEventArgs>(OnDestruction);
        SubscribeLocalEvent<XenoEggComponent, ComponentInit>(OnEggComponentInit);
        SubscribeLocalEvent<NukeDisarmSuccessEvent>(OnNukeDisarm);
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        // return;
        var query = Query;
        while (query.MoveNext(out var uid, out var xeno, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            var minPlayers = xeno.MinPlayers;
            if (!ev.Forced && ev.Players.Length < minPlayers)
            {
                _chatManager.SendAdminAnnouncement(Loc.GetString("xeno-not-enough-ready-players", ("readyPlayersCount", ev.Players.Length), ("minimumPlayers", minPlayers)));
                ev.Cancel();
                continue;
            }

            if (ev.Players.Length != 0)
                continue;

            _chatManager.DispatchServerAnnouncement(Loc.GetString("xeno-no-one-ready"));
            ev.Cancel();
        }
    }

    private void OnEggComponentInit(EntityUid uid, XenoEggComponent component, ComponentInit args)
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

            var eggs = GetCountByTag("XenoEgg");
            var xenos = GetMobAliveCountByTag("Xeno");
            var mariens = GetMobAliveCountByTag("Marien");

            if (eggs >= xeno.WinningXenoEggCount)
            {
                xeno.WinConditions.Add(WinCondition.EggsWinningCount);
                SetWinType(uid, WinType.XenoMinor, xeno);
            }

            if (xenos == 0)
            {
                xeno.WinConditions.Add(WinCondition.AllXenoDied);
                SetWinType(uid, WinType.MarineMajor, xeno);
            }

            if (mariens == 0)
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

    private int GetCountByTag(string tag)
    {
        return EntityManager.EntityQuery<TagComponent>().Where((comp) => comp.Tags.Contains(tag)).Count();
    }

    private int GetMobAliveCountByTag(string tag)
    {
        var count = 0;

        foreach (var (tagComp, mobStateComp) in EntityManager.EntityQuery<TagComponent, MobStateComponent>())
        {
            if (!tagComp.Tags.Contains(tag) || mobStateComp.CurrentState != MobState.Alive)
                continue;

            count++;
        }

        return count;
    }
}
