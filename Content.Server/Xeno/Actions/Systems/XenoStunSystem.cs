using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.Stunnable;
using Content.Shared.Mobs.Components;

namespace Content.Server.Xeno.Systems;

public sealed partial class XenoStunSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoStunComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoStunComponent, XenoStunEvent>(OnStun);
    }

    private void OnStartup(EntityUid uid, XenoStunComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnStun(EntityUid uid, XenoStunComponent comp, XenoStunEvent args)
    {
        if (!HasComp<MobStateComponent>(args.Target) || HasComp<XenoComponent>(args.Target))
            return;

        _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(comp.StunTime), comp.Refresh);
    }
}
