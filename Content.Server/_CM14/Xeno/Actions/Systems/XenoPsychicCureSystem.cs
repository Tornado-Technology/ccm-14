using Content.Server.Body.Systems;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Xeno;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using XenoPsychicCureComponent = Content.Server._CM14.Xeno.Actions.Components.XenoPsychicCureComponent;
using XenoRejuvenateProjComponent = Content.Server._CM14.Xeno.Actions.Components.XenoRejuvenateProjComponent;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoPsychicCureSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoPsychicCureComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoPsychicCureComponent, XenoPsychicCureEvent>(OnPsychicCure);
        SubscribeLocalEvent<XenoPsychicCureComponent, XenoPsychicCureDoAfterEvent>(OnPsychicCureDoAfter);
        SubscribeLocalEvent<XenoRejuvenateProjComponent, StartCollideEvent>(OnCollide);
    }
    private void OnStartup(EntityUid uid, XenoPsychicCureComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnPsychicCureDoAfter(EntityUid uid, XenoPsychicCureComponent component, XenoPsychicCureDoAfterEvent args)
    {
        if (!args.Cancelled)
        {
            if (args.Target != null)
            {
                Heal((EntityUid)args.Target, component.HealAmount);
            }
        }
    }

    private void OnPsychicCure(EntityUid uid, XenoPsychicCureComponent component, XenoPsychicCureEvent args)
    {
        if (!HasComp<XenoComponent>(args.Target) || !HasComp<DamageableComponent>(args.Target))
            return;

        var doAfterEventArgs =
          new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(6.5f), new XenoPsychicCureDoAfterEvent(), uid, target: args.Target, used: uid)
          {
              BreakOnUserMove = true,
              BreakOnTargetMove = true,
              NeedHand = false,
              BreakOnDamage = true,
          };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void Heal(EntityUid target, float healAmount)
    {
        if (!HasComp<XenoComponent>(target) || !TryComp<DamageableComponent>(target, out var damageable))
            return;

        _bloodstreamSystem.TryModifyBleedAmount(target, -healAmount);

        foreach (var type in damageable.Damage.DamageDict.Keys)
        {
            var damageInType = damageable.Damage.DamageDict[type];
            if (damageInType == 0)
                continue;

            _damageableSystem.TryChangeDamage(target, new DamageSpecifier(_proto.Index<DamageTypePrototype>(type), -healAmount), true);
        }
    }

    private void OnCollide(EntityUid uid, XenoRejuvenateProjComponent component, ref StartCollideEvent args)
    {
        Heal(args.OtherEntity, component.HealAmount);
    }
}
