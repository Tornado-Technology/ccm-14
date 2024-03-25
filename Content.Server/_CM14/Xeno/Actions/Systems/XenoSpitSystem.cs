using Content.Server.Weapons.Ranged.Systems;
using Content.Shared._CM14.Xeno;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoSpitSystem : EntitySystem
{
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoSpitComponent, XenoSpitEvent>(OnSpit);
        SubscribeLocalEvent<XenoSpitRejuvenateComponent, XenoSpitRejuvenateEvent>(OnSpitRejuvenate);
    }

    private void OnSpit(EntityUid uid, XenoSpitComponent comp, XenoSpitEvent args)
    {
        Spit(uid, comp.Projectile, comp.Speed, args.Target);
    }

    private void OnSpitRejuvenate(EntityUid uid, XenoSpitRejuvenateComponent comp, XenoSpitRejuvenateEvent args)
    {
        Spit(uid, comp.Projectile, comp.Speed, args.Target);
    }

    private void Spit(EntityUid uid, string proj, float speed, EntityCoordinates target)
    {
        var transform = Transform(uid);
        var projectile = Spawn(proj, transform.Coordinates);
        var direction = target.ToMapPos(EntityManager, _transform) - _transform.GetWorldPosition(transform);
        var userVelocity = _physics.GetMapLinearVelocity(uid);
        _gunSystem.ShootProjectile(projectile, direction, userVelocity, uid, uid, speed);
    }
}
