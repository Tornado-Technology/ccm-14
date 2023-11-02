using Content.Server.Mind.Commands;
using Content.Server.Polymorph.Systems;
using Content.Server.Research.Systems;
using Content.Shared.Actions;
using Content.Shared.Kitchen;
using Content.Shared.Mind;
using Content.Shared.Research.Components;
using Content.Shared.Wieldable;
using Content.Shared.Xeno;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using System.Linq;

namespace Content.Server.Xeno.Components;

public sealed partial class XenoSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;

    private readonly Dictionary<int, int> _tierLimit = new()
    {
        { 0, -1 },
        { 1, -1 },
        { 2, 10 },
        { 3,  5 },
        { 4, -1 },
    };

    private readonly Dictionary<int, int> _tiers = new();

    /// <summary>
    ///     Timer used to avoid updating the UI state every frame (which would be overkill)
    /// </summary>
    private float _updateTimer;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEvolutionsComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoEvolutionsComponent, XenoEvolutionActionEvent>(OnEvolutionMenu);
        SubscribeLocalEvent<XenoEvolutionsComponent, EvolveMessage>(OnEvolve);

        SubscribeLocalEvent<XenoComponent, SharpAfterEvent>(OnSharpAfter);
    }

    private void OnEvolve(EntityUid uid, XenoEvolutionsComponent component, EvolveMessage args)
    {
        PolymorphEntity(uid, args.Evolution.Prototype);
    }

    private void OnStartup(EntityUid uid, XenoEvolutionsComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);

    }

    private void OnEvolutionMenu(EntityUid uid, XenoEvolutionsComponent component, XenoEvolutionActionEvent args)
    {
        if (!TryComp<ActorComponent>(uid, out var actor))
            return;

        _uiSystem.TryToggleUi(uid, XenoEvolutionUiKey.Key, actor.PlayerSession);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _tiers.Clear();

        var query = EntityQueryEnumerator<XenoComponent, XenoEvolutionsComponent, XenoTierComponent>();
        while (query.MoveNext(out var uid, out var xeno, out var evol, out var tiers))
        {
            if (_tiers.TryGetValue(tiers.Tier, out var tier))
            {
                _tiers.Remove(tiers.Tier);
                _tiers.Add(tiers.Tier, tier + 1);
            }
            else
            {
                _tiers.Add(tiers.Tier, 1);
            }

            if (evol.Evolutions == null || evol.Evolutions.Count == 0)
                continue;

            var evolComp = evol.Evolutions.MaxBy((el) => el.Evolution);
            var maxEvolution = evolComp == null ? 0f : evolComp.Evolution;
            var evolution = frameTime * evol.EvolutionModifer;

            evol.Evolution = Math.Min(evol.Evolution + evolution, maxEvolution);
        }

        UpdateEvolutionUI(frameTime);
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
            _uiSystem.TrySetUiState(uid, XenoEvolutionUiKey.Key, state, ui: uiComp);
        }
    }

    private void OnSharpAfter(EntityUid uid, XenoComponent component, SharpAfterEvent args)
    {
        var query = EntityQueryEnumerator<ResearchServerComponent>();
        while (query.MoveNext(out var serverUid, out var server))
        {
            _research.ModifyServerPoints(serverUid, component.ResearchPoints, server);
        }
    }

    private EntityUid? PolymorphEntity(EntityUid uid, string proto)
    {
        if (_uiSystem.IsUiOpen(uid, XenoEvolutionUiKey.Key) && TryComp<ActorComponent>(uid, out var actor))
        {
            _uiSystem.TryClose(uid, XenoEvolutionUiKey.Key, actor.PlayerSession);
        }

        var targetTransformComp = Transform(uid);
        var child = Spawn(proto, targetTransformComp.Coordinates);

        MakeSentientCommand.MakeSentient(child, EntityManager);

        var childXform = Transform(child);
        childXform.LocalRotation = targetTransformComp.LocalRotation;

        if (_containerSystem.TryGetContainingContainer(uid, out var cont))
            cont.Insert(child);

        if (_mindSystem.TryGetMind(uid, out var mindId, out var mind))
            _mindSystem.TransferTo(mindId, child, mind: mind);

        EntityManager.DeleteEntity(uid);
        return child;
    }
}
