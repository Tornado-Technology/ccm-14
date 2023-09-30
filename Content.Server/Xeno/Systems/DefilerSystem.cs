using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Server.Weapons.Ranged.Systems;
using Robust.Server.GameObjects;

namespace Content.Server.Xeno.Systems;


public sealed partial class DefilerSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;



    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DefilerComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<DefilerComponent, DefilerDefaultSpitEvent>(OnDefaultSpit);
        SubscribeLocalEvent<DefilerComponent, DefilerAcidSpitEvent>(OnAcidSpit);
        SubscribeLocalEvent<DefilerComponent, DefilerExplosiveEvent>(OnExsplosive);
    }

    protected void OnStartup(EntityUid uid, DefilerComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.DefilerDefaultSpit);
        _actionsSystem.AddAction(uid, component.DefilerAcidSpit);
        _actionsSystem.AddAction(uid, component.DefilerExplosive);
    }

    private void OnDefaultSpit(EntityUid uid, DefilerComponent comp, DefilerDefaultSpitEvent args)
    {

        var defaultBullet = Spawn("ProjectileDefilerDefaultSpit", Transform(uid).Coordinates);
        var xform = Transform(uid);
        var mapCoords = args.Target.ToMap(EntityManager);
        var direction = mapCoords.Position - xform.MapPosition.Position;
        var userVelocity = _physics.GetMapLinearVelocity(uid);

        _gunSystem.ShootProjectile(defaultBullet, direction, userVelocity, uid, uid);
    }

    private void OnAcidSpit(EntityUid uid, DefilerComponent comp, DefilerAcidSpitEvent args)
    {

        var acidBullet = Spawn("ProjectileDefilerAcidSpit", Transform(uid).Coordinates);
        var xform = Transform(uid);
        var mapCoords = args.Target.ToMap(EntityManager);
        var direction = mapCoords.Position - xform.MapPosition.Position;
        var userVelocity = _physics.GetMapLinearVelocity(uid);

        _gunSystem.ShootProjectile(acidBullet, direction, userVelocity, uid, uid);
    }

    private void OnExsplosive(EntityUid uid, DefilerComponent comp, DefilerExplosiveEvent args)
    {
        var sysMan = IoCManager.Resolve<IEntitySystemManager>();
        sysMan.GetEntitySystem<ExplosionSystem>().QueueExplosion(uid, "Radioactive", 300, 2, 200, 0, 0, false, uid, true);
        _damageableSystem.TryChangeDamage(uid, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 1000));
    }


}
