using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Server.Body.Systems;
using Content.Server.Ghost;
using Robust.Shared.Map;

namespace Content.Server.Xeno.Systems;

public sealed partial class DroneSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoDroneComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoDroneComponent, XenoDroneBuildEvent>(OnBuild);
        SubscribeLocalEvent<XenoDroneComponent, XenoDroneBuildDoAfterEvent>(OnBuildDoAfter);
        SubscribeLocalEvent<XenoDroneComponent, XenoDronePsychicCureEvent>(OnPsychicCure);
        SubscribeLocalEvent<XenoDroneComponent, XenoDronePsychicCureDoAfterEvent>(OnPsychicCureDoAfter);
    }

    private void OnBuildDoAfter(EntityUid uid, XenoDroneComponent component, XenoDroneBuildDoAfterEvent args)
    {
        args.Repeat = false;
        if (args.Handled || args.Cancelled)
            return;

        Spawn(component.BuildingWall, args.Coordinates);
    }

    private void OnPsychicCureDoAfter(EntityUid uid, XenoDroneComponent component, XenoDronePsychicCureDoAfterEvent args)
    {
        args.Repeat = false;
        if (!HasComp<XenoComponent>(args.Target) || !TryComp<DamageableComponent>(args.Target, out var damageable) || args.Handled || args.Cancelled)
        {
            return;
        }

        _bloodstreamSystem.TryModifyBleedAmount((EntityUid)args.Target, -100);

        foreach (var type in damageable.Damage.DamageDict.Keys)
        {
            var damageInType = damageable.Damage.DamageDict[type];
            if (damageInType == 0)
                continue;

            _damageableSystem.TryChangeDamage(args.Target, new DamageSpecifier(_proto.Index<DamageTypePrototype>(type), -100), true);
        }
    }

    private void OnStartup(EntityUid uid, XenoDroneComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.ActionBuild);
        _actionsSystem.AddAction(uid, component.ActionPsychicCure);
    }

    private void OnBuild(EntityUid uid, XenoDroneComponent component, XenoDroneBuildEvent args)
    {
        var doAfterEventArgs =
           new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(10f), new XenoDroneBuildDoAfterEvent(args.Target.ToMap(EntityManager)), uid, target: uid, used: uid)
           {
               BreakOnUserMove = true,
               BreakOnTargetMove = false,
               NeedHand = false,
               BreakOnDamage = true,
           };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void OnPsychicCure(EntityUid uid, XenoDroneComponent component, XenoDronePsychicCureEvent args)
    {
        if (!HasComp<XenoComponent>(args.Target) || !HasComp<DamageableComponent>(args.Target))
            return;

        var doAfterEventArgs =
          new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(10f), new XenoDronePsychicCureDoAfterEvent(), args.Target, target: args.Target, used: uid)
          {
              BreakOnUserMove = true,
              BreakOnTargetMove = true,
              NeedHand = false,
              BreakOnDamage = true,
          };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }
}
