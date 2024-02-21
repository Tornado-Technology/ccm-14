using Content.Server.Actions;
using Content.Shared._CM14.FriendlyFire;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Utility;

namespace Content.Server._CM14.FriendlyFire;

public sealed partial class FriendlyFireSystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly FriendlyFireSystem _friendlyFire = default!;

    private void ToggleableInitialize()
    {
        SubscribeLocalEvent<FriendlyFireToggleableComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<FriendlyFireToggleableComponent, ComponentRemove>(OnRemove);
        SubscribeLocalEvent<FriendlyFireToggleableComponent, ToggleActionEvent>(OnToggleAction);
        SubscribeLocalEvent<FriendlyFireToggleableComponent, GetFireRateEvent>(OnGetFireRate);

        SubscribeLocalEvent<FriendlyFireToggleableActionComponent, ComponentStartup>(OnToggleableStartup);
        SubscribeLocalEvent<FriendlyFireToggleableActionComponent, FriendlyFireToggleableToggleEvent>(OnToggleableToggle);
    }

    private void OnStartup(Entity<FriendlyFireToggleableComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.Action = _actions.AddAction(ent, ent.Comp.Prototype);
    }

    private void OnRemove(Entity<FriendlyFireToggleableComponent> ent, ref ComponentRemove args)
    {
        if (ent.Comp.Action is not { } action)
            return;

        _actions.RemoveAction(ent, action);
    }

    private void OnToggleAction(Entity<FriendlyFireToggleableComponent> ent, ref ToggleActionEvent args)
    {
        if (args.Handled)
            return;

        if (ent.Comp.Action is not { } action)
            return;

        _friendlyFire.Toggle(ent);

        var ev = new FriendlyFireToggleableToggleEvent(_friendlyFire.GetEnabled(ent));
        RaiseLocalEvent(action, ref ev);
    }

    private void OnGetFireRate(Entity<FriendlyFireToggleableComponent> ent, ref GetFireRateEvent args)
    {
        if (_friendlyFire.GetEnabled(ent))
            return;

        args.FireRate *= ent.Comp.FireRateModifier;
    }

    private void OnToggleableStartup(Entity<FriendlyFireToggleableActionComponent> ent, ref ComponentStartup args)
    {
        if (!_actions.TryGetActionData(ent, out var actionComponent))
            return;

        actionComponent.Icon = _friendlyFire.GetEnabled(Transform(ent).ParentUid) ? ent.Comp.IconOn : ent.Comp.IconOff;
        Dirty(ent, actionComponent);
    }

    private void OnToggleableToggle(Entity<FriendlyFireToggleableActionComponent> ent, ref FriendlyFireToggleableToggleEvent args)
    {
        if (!_actions.TryGetActionData(ent, out var actionComponent))
            return;

        actionComponent.Icon = args.Enabled ? ent.Comp.IconOn : ent.Comp.IconOff;
        Dirty(ent, actionComponent);
    }
}
