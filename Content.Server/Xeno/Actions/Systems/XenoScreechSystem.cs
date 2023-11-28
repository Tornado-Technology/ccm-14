using Content.Server.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Shared.Stunnable;
using Content.Shared.Mobs.Components;
using XenoScreechComponent = Content.Server.Xeno.Actions.Components.XenoScreechComponent;

namespace Content.Server.Xeno.Systems;

public sealed partial class XenoScreechSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoScreechComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoScreechComponent, XenoScreechEvent>(OnStun);
    }

    private void OnStartup(EntityUid uid, XenoScreechComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnStun(EntityUid uid, XenoScreechComponent comp, XenoScreechEvent args)
    {
        foreach (var mob in _lookup.GetComponentsInRange<MobStateComponent>(Transform(uid).Coordinates, comp.Radius))
        {
            if (mob == null || HasComp<XenoComponent>(mob.Owner))
                continue;

            _stunSystem.TryParalyze(mob.Owner, TimeSpan.FromSeconds(comp.StunTime), comp.Refresh);
        }
    }
}
