﻿using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Robust.Client.GameObjects;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Mech;

/// <inheritdoc/>
public sealed class MechSystem : SharedMechSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MechComponent, AppearanceChangeEvent>(OnAppearanceChanged);
        SubscribeLocalEvent<MechComponent, MechEntryEvent>(OnMechEntry);
    }

    private void OnAppearanceChanged(EntityUid uid, MechComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!args.Sprite.TryGetLayer((int) MechVisualLayers.Base, out var layer))
            return;

        var state = component.BaseState;
        var drawDepth = DrawDepth.Mobs;
        if (component.BrokenState != null && _appearance.TryGetData<bool>(uid, MechVisuals.Broken, out var broken, args.Component) && broken)
        {
            state = component.BrokenState;
            drawDepth = DrawDepth.SmallMobs;
        }
        else if (component.OpenState != null && _appearance.TryGetData<bool>(uid, MechVisuals.Open, out var open, args.Component) && open)
        {
            state = component.OpenState;
            drawDepth = DrawDepth.SmallMobs;
        }

        layer.SetState(state);
        args.Sprite.DrawDepth = (int) drawDepth;
    }
    private void OnMechEntry(EntityUid uid, MechComponent component, MechEntryEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;
        _audio.PlayPredicted(component.MechEntrySound, uid, args.Args.User);
    }
}
