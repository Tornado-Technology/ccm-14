using Content.Shared._CM14.JobRank;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon.Components;
using Robust.Client.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._CM14.JobRank;

public sealed class JobRankSystem : SharedJobRankSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<JobRankComponent, GetStatusIconsEvent>(OnGetStatusIcon);
    }

    private void OnGetStatusIcon(Entity<JobRankComponent> rank, ref GetStatusIconsEvent args)
    {
        if (!HasComp<JobRankComponent>(_player.LocalPlayer?.ControlledEntity))
            return;

        if (rank.Comp.Icon is not { } icon)
            return;

        args.StatusIcons.Add(_prototype.Index(icon));
    }
}
