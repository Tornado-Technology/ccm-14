using Content.Shared.Actions;
using Content.Shared.FriendlyFire;
using Content.Shared.Hands;
using Content.Shared.Popups;

namespace Content.Server.FriendlyFire;

public sealed class PrecisionShootingSystem : EntitySystem
{
    [Dependency] private readonly FriendlyFireSystem _friendlyFire = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PrecisionShootingComponent, PrecisionShootingEvent>(OnToggle);
        SubscribeLocalEvent<PrecisionShootingComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<PrecisionShootingComponent, GotUnequippedHandEvent>(OnGotUnequippedHand);
    }

    private void OnToggle(Entity<PrecisionShootingComponent> ent, ref PrecisionShootingEvent args)
    {
        if (args.Handled)
            return;

        var parent = Transform(ent).ParentUid;
        SetEnabled(ent, parent, !_friendlyFire.GetEnabled(parent));
        args.Handled = true;
    }

    private void OnGetActions(Entity<PrecisionShootingComponent> ent, ref GetItemActionsEvent args)
    {
        args.AddAction(ref ent.Comp.ActionEntity, ent.Comp.Action);
    }

    private void OnGotUnequippedHand(Entity<PrecisionShootingComponent> ent, ref GotUnequippedHandEvent args)
    {
        var parent = args.User;
        SetEnabled(ent, parent, true);
    }

    private void SetEnabled(Entity<PrecisionShootingComponent> ent, EntityUid parent, bool state)
    {
        if (ent.Comp.Enabled == state)
            return;

        _friendlyFire.SetEnabled(parent, state);
        ent.Comp.Enabled = state;
        _popup.PopupEntity(state ? "Вы перестаете целится." : "Вы прицеливаетесь.", parent);
    }
}
