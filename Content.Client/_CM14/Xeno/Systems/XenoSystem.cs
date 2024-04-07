using Content.Client._CM14.Xeno.Components;
using Content.Client._CM14.Xeno.Overlays;
using Content.Shared._CM14.Xeno;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._CM14.Xeno.Systems;

public sealed class XenoSystem : EntitySystem
{
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly SharedEyeSystem _eyeSystem = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoVisionComponent, XenoNightVisionEvent>(OnNightVisionToggle);
        SubscribeLocalEvent<XenoVisionComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<XenoVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<XenoVisionComponent, RoundRestartCleanupEvent>(OnRoundRestart);
    }

    private void OnNightVisionToggle(EntityUid uid, XenoVisionComponent component, XenoNightVisionEvent args)
    {
        if (args.Handled)
            return;

        component.Enabled = !component.Enabled;

        UpdateVision((uid, component));
    }

    private void OnPlayerAttached(EntityUid uid, XenoVisionComponent component, PlayerAttachedEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;
        if (args.Entity != uid)
            return;
        component.Enabled = true;
        UpdateVision((uid, component));
    }

    private void OnPlayerDetached(EntityUid uid, XenoVisionComponent component, PlayerDetachedEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;
        if (args.Entity != uid)
            return;
        component.Enabled = false;
        UpdateVision((uid, component));
    }

    private void OnRoundRestart(EntityUid uid, XenoVisionComponent component, RoundRestartCleanupEvent args)
    {
        if (_player.LocalSession == null)
            return;
        if (_player.LocalSession.AttachedEntity != uid)
            return;

        component.Enabled = false;
        UpdateVision((uid, component));
    }

    private void UpdateVision(Entity<XenoVisionComponent> ent)
    {
        _eyeSystem.SetDrawLight(ent.Owner, !ent.Comp.Enabled);
        _eyeSystem.SetDrawFov(ent.Owner, !ent.Comp.Enabled);
        _lightManager.DrawLighting = !ent.Comp.Enabled;
        _lightManager.DrawHardFov = !ent.Comp.Enabled;

        if (ent.Comp.Enabled)
        {
            ent.Comp.VisionOverlay ??= new XenoVisionOverlay();
            _overlayMan.AddOverlay(ent.Comp.VisionOverlay);
        }
        else
        {
            if (ent.Comp.VisionOverlay != null)
                _overlayMan.RemoveOverlay(ent.Comp.VisionOverlay);
        }
    }
}
