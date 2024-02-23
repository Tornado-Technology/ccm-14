using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Events;
using Robust.Shared.Serialization;
using XenoRestComponent = Content.Shared._CM14.Xeno.Components.XenoRestComponent;
using XenoRestEvent = Content.Shared._CM14.Xeno.Events.XenoRestEvent;
using XenoRestingComponent = Content.Shared._CM14.Xeno.Components.XenoRestingComponent;

namespace Content.Shared._CM14.Xeno.Systems;

public sealed class XenoRestSystem : EntitySystem
{
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoRestComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoRestComponent, XenoRestEvent>(OnRest);
        SubscribeLocalEvent<XenoRestComponent, InteractionAttemptEvent>(OnInteract);
        SubscribeLocalEvent<XenoRestingComponent, UpdateCanMoveEvent>(OnCanMove);
    }

    private void OnStartup(Entity<XenoRestComponent> rest, ref ComponentStartup args)
    {
        _actions.AddAction(rest, rest.Comp.Action);
    }

    private void OnInteract(Entity<XenoRestComponent> rest, ref InteractionAttemptEvent args)
    {
        if (rest.Comp.IsInRest)
            args.Cancel();
    }

    private void OnRest(Entity<XenoRestComponent> rest, ref XenoRestEvent args)
    {
        if (args.Handled)
            return;

        if (HasComp<XenoRestingComponent>(rest))
        {
            RemComp<XenoRestingComponent>(rest);
            _appearance.SetData(rest, XenoVisualLayers.Base, XenoRestState.NotResting);
            rest.Comp.IsInRest = false;
        }
        else
        {
            AddComp<XenoRestingComponent>(rest);
            _appearance.SetData(rest, XenoVisualLayers.Base, XenoRestState.Resting);
            rest.Comp.IsInRest = true;
        }

        args.Handled = true;
        _actionBlocker.UpdateCanMove(rest);
    }

    private void OnCanMove(Entity<XenoRestingComponent> rest, ref UpdateCanMoveEvent args)
    {
        args.Cancel();
    }
}

[Serializable, NetSerializable]
public enum XenoVisualLayers : byte
{
    Base
}

[Serializable, NetSerializable]
public enum XenoRestState : byte
{
    NotResting,
    Resting
}
