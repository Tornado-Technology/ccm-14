using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Movement.Components;
using Content.Shared.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoEndureSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoEndureComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoEndureComponent, XenoEndureEvent>(OnToggle);
    }

    private void OnStartup(EntityUid uid, XenoEndureComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
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
