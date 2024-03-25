using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoScreechSystem : EntitySystem
{
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoScreechComponent, XenoScreechEvent>(OnStun);
    }

    private void OnStun(EntityUid uid, XenoScreechComponent comp, XenoScreechEvent args)
    {
        foreach (var mob in _lookup.GetComponentsInRange<MobStateComponent>(Transform(uid).MapPosition, comp.Radius))
        {
            if (mob == null || HasComp<XenoComponent>(mob.Owner))
                continue;

            _stunSystem.TryParalyze(mob.Owner, TimeSpan.FromSeconds(comp.StunTime), comp.Refresh);
        }
    }
}
