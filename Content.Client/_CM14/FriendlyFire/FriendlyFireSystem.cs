using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Ghost;
using Robust.Client.Player;
using Robust.Shared.Prototypes;
using Content.Shared._CM14.FriendlyFire;
using Content.Shared.CombatMode;
using Content.Shared.Hands.Components;
using Content.Shared.Weapons.Ranged.Components;

namespace Content.Client.FriendlyFire;

public sealed class FriendlyFireSystem : SharedStatusIconSystem
{


    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FriendlyFireComponent, GetStatusIconsEvent>(OnGetStatusIcon);
    }

    private void GetStatusIcon(string statusIcon, ref GetStatusIconsEvent args)
    {
        var ent = _player.LocalPlayer?.ControlledEntity;

        if (!HasComp<FriendlyFireComponent>(ent) && !HasComp<GhostComponent>(ent))
            return;


        args.StatusIcons.Add(_prototype.Index<StatusIconPrototype>(statusIcon));
    }

    private void OnGetStatusIcon(EntityUid uid, FriendlyFireComponent component, ref GetStatusIconsEvent args)
    {
        // Dont set icon if not in combat mode.
        if (!TryComp<CombatModeComponent>(uid, out var combatMode) || !combatMode.IsInCombatMode)
            return;

        // Dont set icon if not carrying weapon
        if (!TryComp<HandsComponent>(uid, out var hands))
            return;

        var itemInHands = hands?.ActiveHand?.HeldEntity;
        if (itemInHands == null || !HasComp<GunComponent>(itemInHands))
            return;

        if (component.Enabled)
            GetStatusIcon(component.EnableStatusIcon, ref args);
        else
            GetStatusIcon(component.DisableStatusIcon, ref args);
    }
}
