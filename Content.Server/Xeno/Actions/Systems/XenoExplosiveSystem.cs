using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Server.Xeno.Actions.Components;
using Content.Shared.Damage;
using Content.Server.Explosion.EntitySystems;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;

namespace Content.Server.Xeno.Systems;

public sealed partial class XenoExplosiveSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly ExplosionSystem _explosionSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoExplosiveComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoExplosiveComponent, XenoExplosiveEvent>(OnExsplosive);
    }

    private void OnStartup(EntityUid uid, XenoExplosiveComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnExsplosive(EntityUid uid, XenoExplosiveComponent comp, XenoExplosiveEvent args)
    {
        if (!TryComp<MobStateComponent>(uid, out var mobState) || mobState.CurrentState == MobState.Dead)
            return;

        // TODO: Move magick numbers to XenoExplosiveComponent
        _explosionSystem.QueueExplosion(uid, "Radioactive", 300, 2, 200, 0, 0, false, uid, true);
        _damageableSystem.TryChangeDamage(uid, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 1000));
    }
}
