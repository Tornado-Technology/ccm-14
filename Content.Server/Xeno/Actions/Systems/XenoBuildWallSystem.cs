using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.DoAfter;
using Content.Server.Xeno.Actions.Components;

namespace Content.Server.Xeno.Systems;

public sealed class XenoBuildWallSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoBuildWallComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoBuildWallComponent, XenoBuildWallEvent>(OnBuild);
        SubscribeLocalEvent<XenoBuildWallComponent, XenoBuildWallDoAfterEvent>(OnBuildDoAfter);
    }

    private void OnStartup(EntityUid uid, XenoBuildWallComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnBuild(EntityUid uid, XenoBuildWallComponent component, XenoBuildWallEvent args)
    {
        var doAfterEventArgs =
           new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(component.TimeUsage), new XenoBuildWallDoAfterEvent(args.Target.ToMap(EntityManager)), uid, target: uid, used: uid)
           {
               BreakOnUserMove = true,
               BreakOnTargetMove = false,
               NeedHand = false,
               BreakOnDamage = true,
           };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void OnBuildDoAfter(EntityUid uid, XenoBuildWallComponent component, XenoBuildWallDoAfterEvent args)
    {
        args.Repeat = false;
        if (args.Handled || args.Cancelled)
            return;

        Spawn(component.WallPrototype, args.Coordinates);
    }
}
