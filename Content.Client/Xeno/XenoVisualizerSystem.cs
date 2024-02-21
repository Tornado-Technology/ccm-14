using Content.Shared._CM14.Xeno.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;
using  Content.Shared._CM14.Xeno.Components;

namespace Content.Client.Xeno;

public sealed class XenoVisualizerSystem : VisualizerSystem<XenoRestComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, XenoRestComponent component, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;

        if (sprite is not { BaseRSI: { } rsi } || !sprite.LayerMapTryGet(XenoVisualLayers.Base, out var layer))
        {
            return;
        }

        var state = CompOrNull<MobStateComponent>(uid)?.CurrentState;

        switch (state)
        {
            case MobState.Critical:
                ClearDrawDepth((uid, component, sprite));

                if (rsi.TryGetState("crit", out _))
                    sprite.LayerSetState(layer, "crit");
                break;
            case MobState.Dead:
                SetDrawDepth((uid, component, sprite));

                if (rsi.TryGetState("dead", out _))
                    sprite.LayerSetState(layer, "dead");
                break;
            default:
                ClearDrawDepth((uid, component, sprite));

                if (args.AppearanceData.TryGetValue(XenoVisualLayers.Base, out var resting) &&
                    resting is XenoRestState.Resting)
                {
                    if (rsi.TryGetState("sleeping", out _))
                        sprite.LayerSetState(layer, "sleeping");
                    break;
                }

                if (rsi.TryGetState("running", out _))
                    sprite.LayerSetState(layer, "running");
                break;
        }
    }

    private void SetDrawDepth(Entity<XenoRestComponent, SpriteComponent> xeno)
    {
        if (xeno.Comp2.DrawDepth > (int) DrawDepth.DeadMobs)
            return;

        xeno.Comp1.OriginalDrawDepth = xeno.Comp2.DrawDepth;
        xeno.Comp2.DrawDepth = (int)DrawDepth.DeadMobs;
    }

    private void ClearDrawDepth(Entity<XenoRestComponent, SpriteComponent> xeno)
    {
        if (xeno.Comp1.OriginalDrawDepth == null)
            return;

        xeno.Comp2.DrawDepth = xeno.Comp1.OriginalDrawDepth.Value;
        xeno.Comp1.OriginalDrawDepth = null;
    }
}
