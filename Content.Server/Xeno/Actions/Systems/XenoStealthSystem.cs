using Content.Server.Weapons.Ranged.Systems;
using Content.Server.Xeno.Actions.Components;
using Content.Shared.Actions;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Content.Shared.Xeno;

namespace Content.Server.Xeno.Actions.Systems;

public sealed class XenoStealthSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedStealthSystem _stealth = default!;

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

        if (!component.Enabled && HasComp<StealthComponent>(uid))
            RemComp<StealthComponent>(uid);
        else
            EnsureComp<StealthComponent>(uid);
    }
}
