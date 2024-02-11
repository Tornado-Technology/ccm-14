using Content.Shared.Actions;
using Content.Shared.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoLayEggSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

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
