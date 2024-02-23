using Content.Shared.Actions;
using Content.Shared.Stealth.Components;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoStealthSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoStealthComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoStealthComponent, XenoToggleStealthEvent>(OnToggle);
    }

    private void OnStartup(EntityUid uid, XenoStealthComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnToggle(EntityUid uid, XenoStealthComponent component, XenoToggleStealthEvent args)
    {
        component.Enabled = !component.Enabled;
        EnsureComp<StealthComponent>(uid).Enabled = component.Enabled;
    }
}
