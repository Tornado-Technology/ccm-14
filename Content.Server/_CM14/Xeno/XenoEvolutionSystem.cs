using System.Linq;
using Content.Server.Mind.Commands;
using Content.Shared.Actions;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Xeno;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Player;

namespace Content.Server._CM14.Xeno;

public sealed class XenoEvolutionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    private readonly Dictionary<int, int> _tiers = new();
    private readonly Dictionary<int, int> _tierLimit = new()
    {
        { 0, -1 },
        { 1, -1 },
        { 2, 10 },
        { 3,  5 },
        { 4, -1 },
        { 5, -1 },
    };

    private float _updateTimer;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEvolutionsComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoEvolutionsComponent, XenoEvolutionActionEvent>(OnMenu);
        SubscribeLocalEvent<XenoEvolutionsComponent, EvolveMessage>(OnEvolve);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _tiers.Clear();

        var query = EntityQueryEnumerator<XenoComponent, XenoEvolutionsComponent, MobStateComponent, XenoTierComponent>();
        while (query.MoveNext(out var uid, out var xeno, out var evol, out var state,  out var tiers))
        {
            evol.EvolutionModifer = XenoEvolutionsComponent.BaseEvolutionModifer;
            if (xeno.OnResting)
            {
                evol.EvolutionModifer += xeno.OnWeeds ? 0.1f : 0.05f;
            }

            if (state.CurrentState != MobState.Alive)
                continue;

            if (_tiers.TryGetValue(tiers.Tier, out var tier))
            {
                _tiers.Remove(tiers.Tier);
                _tiers.Add(tiers.Tier, tier + 1);
            }
            else
            {
                _tiers.Add(tiers.Tier, 1);
            }

            if (evol.Evolutions.Count == 0)
                continue;

            var evolComp = evol.Evolutions.MaxBy((el) => el.Evolution);
            var maxEvolution = evolComp?.Evolution ?? 0f;
            var evolution = frameTime * evol.EvolutionModifer;

            evol.Evolution = Math.Min(evol.Evolution + evolution, maxEvolution);
        }

        UpdateEvolutionUI(frameTime);
    }

    private void OnStartup(Entity<XenoEvolutionsComponent> ent, ref ComponentStartup args)
    {
        _actions.AddAction(ent, ent.Comp.Action);
    }

    private void OnMenu(Entity<XenoEvolutionsComponent> ent, ref XenoEvolutionActionEvent args)
    {
        if (!TryComp<ActorComponent>(ent, out var actor))
            return;

        _ui.TryToggleUi(ent, XenoEvolutionUiKey.Key, actor.PlayerSession);
    }

    private void OnEvolve(Entity<XenoEvolutionsComponent> ent, ref EvolveMessage args)
    {
        PolymorphEntity(ent, args.Evolution.Prototype);
    }

    private EntityUid? PolymorphEntity(EntityUid uid, string proto)
    {
        if (_ui.IsUiOpen(uid, XenoEvolutionUiKey.Key) && TryComp<ActorComponent>(uid, out var actor))
        {
            _ui.TryClose(uid, XenoEvolutionUiKey.Key, actor.PlayerSession);
        }

        var targetTransformComp = Transform(uid);
        var child = Spawn(proto, targetTransformComp.Coordinates);

        MakeSentientCommand.MakeSentient(child, EntityManager);

        var childXform = Transform(child);
        childXform.LocalRotation = targetTransformComp.LocalRotation;

        if (_container.TryGetContainingContainer(uid, out var cont))
            _container.Insert(child, cont);

        if (_mind.TryGetMind(uid, out var mindId, out var mind))
            _mind.TransferTo(mindId, child, mind: mind);

        EntityManager.DeleteEntity(uid);
        return child;
    }

    private void UpdateEvolutionUI(float frameTime)
    {
        _updateTimer += frameTime;

        if (_updateTimer < 1)
            return;

        _updateTimer -= 1;

        var query = EntityQueryEnumerator<XenoComponent, XenoEvolutionsComponent, UserInterfaceComponent>();
        while (query.MoveNext(out var uid, out var _, out var evolution, out var uiComp))
        {
            var state = new XenoEvolutionBoundInterfaceState(evolution.Evolution, evolution.EvolutionModifer, evolution.Evolutions, evolution.Enabled, _tierLimit, _tiers);
            _ui.TrySetUiState(uid, XenoEvolutionUiKey.Key, state, ui: uiComp);
        }
    }
}
