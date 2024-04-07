using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._CM14.XenoVision;

public sealed class XenoVisionSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoVisionComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<XenoVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<XenoVisionComponent, RoundRestartCleanupEvent>(OnRoundRestart);
    }

    private void OnPlayerAttached(EntityUid uid, XenoVisionComponent component, PlayerAttachedEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;
        if (args.Entity != uid)
            return;

        ToggleVision(component, true);
    }

    private void OnPlayerDetached(EntityUid uid, XenoVisionComponent component, PlayerDetachedEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;
        if (args.Entity != uid)
            return;

        ToggleVision(component, false);
    }

    private void OnRoundRestart(EntityUid uid, XenoVisionComponent component, RoundRestartCleanupEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;

        ToggleVision(component, false);
    }

    private void ToggleVision(XenoVisionComponent component, bool enabled)
    {
        if (enabled)
        {
            component.VisionOverlay ??= new XenoVisionOverlay();
            _overlayMan.AddOverlay(component.VisionOverlay);
        }
        else
        {
            if (component.VisionOverlay != null)
                _overlayMan.RemoveOverlay(component.VisionOverlay);
        }
    }
}
