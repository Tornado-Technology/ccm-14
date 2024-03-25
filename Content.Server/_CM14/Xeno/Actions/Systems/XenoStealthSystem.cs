using Content.Shared.Stealth.Components;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoStealthSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoStealthComponent, XenoToggleStealthEvent>(OnToggle);
    }

    private void OnToggle(EntityUid uid, XenoStealthComponent component, XenoToggleStealthEvent args)
    {
        component.Enabled = !component.Enabled;
        EnsureComp<StealthComponent>(uid).Enabled = component.Enabled;
    }
}
