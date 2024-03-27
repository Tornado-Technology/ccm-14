using System.Numerics;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;
using Content.Shared.StatusEffect;
using Robust.Server.GameObjects;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoStunSystem : EntitySystem
{
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoStunComponent, XenoStunEvent>(OnStun);
    }

    private void OnStun(Entity<XenoStunComponent> stun, ref XenoStunEvent args)
    {
        var xform = Transform(stun);

        var targetMap = args.Target.GetMapId(EntityManager);
        var targetCoords = args.Target.ToMapPos(EntityManager, _transform);

        if (targetMap != xform.MapID)
            return;

        var query = EntityQueryEnumerator<MobStateComponent, StatusEffectsComponent>();
        while (query.MoveNext(out var uid, out _, out var statusEffects))
        {
            if (HasComp<XenoComponent>(uid))
                continue;

            var tempDistance = (targetCoords - _transform.GetWorldPosition(uid)).Length();
            if (tempDistance > stun.Comp.DistanceTolerance)
                continue;

            _stun.TryParalyze(uid, TimeSpan.FromSeconds(stun.Comp.StunTime), stun.Comp.Refresh, statusEffects);
            break;
        }
    }
}
