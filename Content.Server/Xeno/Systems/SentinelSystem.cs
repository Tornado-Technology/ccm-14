using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Server.Weapons.Ranged.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Xeno.Systems;


public sealed partial class SentinelSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly ExplosionSystem _explosionSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SentinelComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<SentinelComponent, SentinelDefaultSpitEvent>(OnDefaultSpit);
    }

    protected void OnStartup(EntityUid uid, SentinelComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.SentinelDefaultSpit);
    }

    private void OnDefaultSpit(EntityUid uid, SentinelComponent comp, SentinelDefaultSpitEvent args)
    {
        Spit(uid, args.Target, "ProjectileSentinelDefaultSpit");
    }

    private void Spit(EntityUid uid, EntityCoordinates target, string projectile)
    {
        var acidBullet = Spawn(projectile, Transform(uid).Coordinates);
        var xform = Transform(uid);
        var mapCoords = target.ToMap(EntityManager);
        var direction = mapCoords.Position - xform.MapPosition.Position;
        var userVelocity = _physics.GetMapLinearVelocity(uid);

        _gunSystem.ShootProjectile(acidBullet, direction, userVelocity, uid, uid);
    }
}
