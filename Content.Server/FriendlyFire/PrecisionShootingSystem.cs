using Content.Shared.Actions;
using Content.Shared.FriendlyFire;

namespace Content.Server.FriendlyFire;

public sealed class PrecisionShootingSystem : EntitySystem
{
    [Dependency] private readonly FriendlyFireSystem _friendlyFire = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PrecisionShootingComponent, PrecisionShootingEvent>(OnToggle);
        SubscribeLocalEvent<PrecisionShootingComponent, GetItemActionsEvent>(OnGetActions);
    }

    private void OnToggle(Entity<PrecisionShootingComponent> ent, ref PrecisionShootingEvent args)
    {
        if (args.Handled)
            return;

        var parent = Transform(ent).ParentUid;
        _friendlyFire.SetEnabled(parent, !_friendlyFire.GetEnabled(parent));
        ent.Comp.Enabled = _friendlyFire.GetEnabled(parent);
        args.Handled = true;
    }

    private void OnGetActions(Entity<PrecisionShootingComponent> ent, ref GetItemActionsEvent args)
    {
        args.AddAction(ref ent.Comp.ActionEntity, ent.Comp.Action);
    }
}
