using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Actions;
using Content.Shared._CM14.Xeno;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Content.Server._CM14.Xeno.Actions.Components;


namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoSpitSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoSpitComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoSpitComponent, XenoSpitEvent>(OnSpit);

        SubscribeLocalEvent<XenoSpit2Component, ComponentStartup>(OnStartup2);
        SubscribeLocalEvent<XenoSpit2Component, XenoSpit2Event>(OnSpit2);

        SubscribeLocalEvent<XenoSpitRejuvenateComponent, ComponentStartup>(OnStartupRejuvenate);
        SubscribeLocalEvent<XenoSpitRejuvenateComponent, XenoSpitRejuvenateEvent>(OnSpitRejuvenate);
    }

    private void OnStartup(EntityUid uid, XenoSpitComponent component, ComponentStartup args)
    {
        Starturp(uid, component.Action);
    }

    private void OnSpit(EntityUid uid, XenoSpitComponent comp, XenoSpitEvent args)
    {
        Spit(uid, comp.Projectile, comp.Speed, args.Target);
    }

    private void OnStartup2(EntityUid uid, XenoSpit2Component component, ComponentStartup args)
    {
        Starturp(uid, component.Action);
    }

    private void OnSpit2(EntityUid uid, XenoSpit2Component comp, XenoSpit2Event args)
    {
        Spit(uid, comp.Projectile, comp.Speed, args.Target);
    }

    private void OnStartupRejuvenate(EntityUid uid, XenoSpitRejuvenateComponent component, ComponentStartup args)
    {
        Starturp(uid, component.Action);
    }

    private void OnSpitRejuvenate(EntityUid uid, XenoSpitRejuvenateComponent comp, XenoSpitRejuvenateEvent args)
    {
        Spit(uid, comp.Projectile, comp.Speed, args.Target);
    }

    private void Starturp(EntityUid uid, string action)
    {
        _actionsSystem.AddAction(uid, action);
    }

    private void Spit(EntityUid uid, string proj, float speed, EntityCoordinates target)
    {
        var transform = Transform(uid);
        var projectile = Spawn(proj, transform.Coordinates);
        var mapCoords = target.ToMap(EntityManager);
        var direction = mapCoords.Position - transform.MapPosition.Position;
        var userVelocity = _physics.GetMapLinearVelocity(uid);
        _gunSystem.ShootProjectile(projectile, direction, userVelocity, uid, uid, speed);
    }
}
