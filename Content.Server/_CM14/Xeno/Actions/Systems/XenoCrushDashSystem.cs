using Content.Server.Disposal.Unit.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Prototypes;
using Content.Server._CM14.Xeno.Actions.Components;
using Content.Shared._CM14.Xeno;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoCrushDashSystem : EntitySystem
{
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoCrushDashComponent, XenoCrushDashEvent>(OnDash);
        SubscribeLocalEvent<XenoCrushDashComponent, ThrowDoHitEvent>(OnDashHit);
    }

    private void OnDash(EntityUid uid, XenoCrushDashComponent component, XenoCrushDashEvent args)
    {
        _throwing.TryThrow(uid, args.Target, component.DashForce);
    }

    private void OnDashHit(EntityUid uid, XenoCrushDashComponent component, ThrowDoHitEvent args)
    {
        if (HasComp<XenoComponent>(args.Target)) // Turn off friendly fire.
            return;

        // TODO: Move magick numbers to XenoCrushDashComponent
        if (HasComp<MobStateComponent>(args.Target))
        {
            _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(component.StunTime), true);
            _damageableSystem.TryChangeDamage(args.Target,
                new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 35));
            return;
        }

        if (HasComp<DisposalUnitComponent>(args.Target))
        {
            _damageableSystem.TryChangeDamage(args.Target,
                new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 2000));
            return;
        }

        _damageableSystem.TryChangeDamage(args.Target,
            new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 300));
    }
}
