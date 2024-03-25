using System.Numerics;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoStunSystem : EntitySystem
{
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly SharedTransformSystem _entTransformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoStunComponent, XenoStunEvent>(OnStun);
    }

    private void OnStun(EntityUid uid, XenoStunComponent comp, XenoStunEvent args)
    {
        // much more optimization lmao
        var sqrDistance = comp.DistanceTolerance * comp.DistanceTolerance;
        if (!TryComp<TransformComponent>(uid, out var xform))
        {
            return;
        }

        var query = EntityQueryEnumerator<MobStateComponent>();
        while (query.MoveNext(out var tempUid, out _))
        {
            if (args.Target.GetMapId(_entManager) != xform.MapID || HasComp<XenoComponent>(tempUid)) continue;

            var tempDistance = Vector2.DistanceSquared(args.Target.ToMapPos(_entManager, _entTransformSystem),
                                                                _entTransformSystem.GetWorldPosition(tempUid));

            if (tempDistance < sqrDistance)
            {
                _stunSystem.TryParalyze(tempUid, TimeSpan.FromSeconds(comp.StunTime), comp.Refresh);
                break;
            }
        }
    }
}
