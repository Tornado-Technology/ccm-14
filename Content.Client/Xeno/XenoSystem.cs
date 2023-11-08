using Content.Shared.Xeno;
using Robust.Client.Graphics;

namespace Content.Client.Xeno;

public sealed partial class XenoSystem : EntitySystem
{
    [Dependency] ILightManager _lightManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoComponent, XenoNightVisionEvent>(OnNightVisionToggle);
    }

    private void OnNightVisionToggle(EntityUid uid, XenoComponent component, XenoNightVisionEvent args)
    {
        if (args.Handled)
            return;

        _lightManager.Enabled = !_lightManager.Enabled;
        args.Handled = true;
    }
}
