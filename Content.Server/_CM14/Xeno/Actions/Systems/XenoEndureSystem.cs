using Content.Shared.Damage;
using Content.Shared.Movement.Components;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoEndureSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEndureComponent, XenoEndureEvent>(OnToggle);
    }

    private void OnToggle(EntityUid uid, XenoEndureComponent component, XenoEndureEvent args)
    {
        component.Enabled = !component.Enabled;

        if (!TryComp<DamageableComponent>(uid, out var damageable))
            return;

        if (component.Enabled)
        {
            damageable.DamageModifierSetId = component.ActiveModifierSet;
            RemComp<InputMoverComponent>(uid);
            return;
        }

        EnsureComp<InputMoverComponent>(uid);
        damageable.DamageModifierSetId = component.PassiveModifierSet;
    }
}
