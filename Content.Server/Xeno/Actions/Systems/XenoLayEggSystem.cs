using Content.Shared.Xeno.Components;
using Content.Shared.Actions;
using Content.Shared.Xeno;

namespace Content.Server.Xeno.Actions.Systems;

public sealed class XenoLayEggSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoLayEggComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoLayEggComponent, XenoLayEggEvent>(ActionUsed);
    }

    private void OnStartup(EntityUid uid, XenoLayEggComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void ActionUsed(EntityUid uid, XenoLayEggComponent action, XenoLayEggEvent args)
    {
        var transform = Transform(uid);
        if (transform.GridUid == null)
            return;

        Spawn(action.EggPrototype, transform.Coordinates);
    }
}
