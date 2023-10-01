using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.Throwing;
using Content.Shared.Stunnable;
using Content.Shared.Mobs.Components;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Server.Disposal.Unit.Components;


namespace Content.Server.Xeno.Systems;

public sealed partial class XenoStunSystem : EntitySystem
{

    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoStunComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoStunComponent, XenoStunEvent>(OnStun);
    }

    protected void OnStartup(EntityUid uid, XenoStunComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.XenoStun);
    }

    private void OnStun(EntityUid uid, XenoStunComponent comp, XenoStunEvent args)
    {
        if (!HasComp<MobStateComponent>(args.Target) || HasComp<XenoComponent>(args.Target)) // todo add xeno component
            return;
        _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(7f), true);
    }

}
