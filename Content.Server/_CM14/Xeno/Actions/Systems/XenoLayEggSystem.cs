using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoLayEggSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoLayEggComponent, XenoLayEggEvent>(ActionUsed);
    }

    private void ActionUsed(EntityUid uid, XenoLayEggComponent action, XenoLayEggEvent args)
    {
        var transform = Transform(uid);
        if (transform.GridUid == null)
            return;

        Spawn(action.EggPrototype, transform.Coordinates);
    }
}
