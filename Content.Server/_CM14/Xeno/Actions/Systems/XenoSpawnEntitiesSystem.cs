using Content.Shared.Actions;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoSpawnEntitiesSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoSpawnEntitiesComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoSpawnEntitiesComponent, ActionXenoSpawnEvent>(OnSpawnEntities);
    }

    private void OnStartup(EntityUid uid, XenoSpawnEntitiesComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnSpawnEntities(EntityUid uid, XenoSpawnEntitiesComponent comp, ActionXenoSpawnEvent args)
    {
        if (!TryComp<MobStateComponent>(uid, out var mobState) || mobState.CurrentState == MobState.Dead)
            return;

        if (!TryComp<TransformComponent>(uid, out var xform))
            return;

        for (var i = 0; i < comp.Count; ++i)
        {
            EntityManager.SpawnAtPosition(comp.Entity, xform.Coordinates);
        }
    }
}
