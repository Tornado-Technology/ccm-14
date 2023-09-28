using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.Throwing;
using Content.Shared.Stunnable;
using Content.Shared.Mobs.Components;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;


namespace Content.Server.Xeno.Systems;

public sealed partial class CrusherSystem : EntitySystem
{

    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CrusherComponent, ComponentStartup>(OnStartup);

        SubscribeLocalEvent<CrusherComponent, CrusherJumpEvent>(OnJump);
        SubscribeLocalEvent<CrusherComponent, ThrowDoHitEvent>(OnJumpHit);

        SubscribeLocalEvent<CrusherComponent, CrusherStunEvent>(OnStun);
    }

    protected void OnStartup(EntityUid uid, CrusherComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.CrusherJump);
        _actionsSystem.AddAction(uid, component.CrusherStun);
    }

    private void OnJump(EntityUid uid, CrusherComponent comp, CrusherJumpEvent args)
    {
        _throwing.TryThrow(uid, args.Target, 5f);
    }
    private void OnJumpHit(EntityUid uid, CrusherComponent comp, ThrowDoHitEvent args)
    {
        if (HasComp<MobStateComponent>(args.Target))
        {
            _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(7f), true);
            _damageableSystem.TryChangeDamage(args.Target, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 35));
        }
        else
            _damageableSystem.TryChangeDamage(args.Target, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 100));

    }

    private void OnStun(EntityUid uid, CrusherComponent comp, CrusherStunEvent args)
    {
        if (!HasComp<MobStateComponent>(args.Target)) // todo add xeno component
            return;
        _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(7f), true);
    }

}
