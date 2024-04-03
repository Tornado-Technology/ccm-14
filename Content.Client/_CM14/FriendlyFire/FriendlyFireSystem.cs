using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Ghost;
using Robust.Client.Player;
using Robust.Shared.Prototypes;
using Content.Shared._CM14.FriendlyFire;

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
        if (component.Enabled)
            GetStatusIcon(component.EnableStatusIcon, ref args);
        else
            GetStatusIcon(component.DisableStatusIcon, ref args);
    }
}









