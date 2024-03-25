using Content.Shared.Damage;
using Robust.Shared.Timing;
using Content.Shared._CM14.Xeno;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoRageSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoRageComponent, XenoRageEvent>(OnToggle);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<XenoRageComponent, DamageableComponent>();
        while (query.MoveNext(out var uid, out var comp, out var damageable))
        {
            if (!comp.Enabled)
                continue;

            if (_timing.CurTime <= comp.TimeUsed)
                continue;

            comp.Enabled = false;
            Dirty(uid, comp);
            damageable.DamageModifierSetId = comp.PassiveModifierSet;
        }
    }

    private void OnToggle(EntityUid uid, XenoRageComponent component, XenoRageEvent args)
    {
        if (!TryComp<DamageableComponent>(uid, out var damageable))
            return;

        component.Enabled = true;
        Dirty(uid, component);
        component.TimeUsed = _timing.CurTime + TimeSpan.FromSeconds(10f);
        damageable.DamageModifierSetId = component.ModifierSet;
    }
}
