using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowXenoAgeSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private StatusIconPrototype _youngIcon = default!;
    private StatusIconPrototype _matureIcon = default!;
    private StatusIconPrototype _elderIcon = default!;
    private StatusIconPrototype _ancientIcon = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShowXenoAgeIconsComponent, GetStatusIconsEvent>(OnGetStatusIcon);

        _youngIcon = _prototype.Index<StatusIconPrototype>("JobRankXenoYoung");
        _matureIcon = _prototype.Index<StatusIconPrototype>("JobRankXenoMature");
        _elderIcon = _prototype.Index<StatusIconPrototype>("JobRankXenoElder");
        _ancientIcon = _prototype.Index<StatusIconPrototype>("JobRankXenoAncient");
    }

    private void OnGetStatusIcon(EntityUid uid, ShowXenoAgeIconsComponent component, ref GetStatusIconsEvent args)
    {
        if (!HasComp<ShowXenoAgeIconsComponent>(_player.LocalPlayer?.ControlledEntity))
            return;

        if (component.OverallRoleTime < 1200)        // 20 hours
        {
            args.StatusIcons.Add(_youngIcon);
            return;
        }

        if (component.OverallRoleTime < 3000)       // 50 hours
        {
            args.StatusIcons.Add(_matureIcon);
            return;
        }

        if (component.OverallRoleTime < 6000)       // 100 hours
        {
            args.StatusIcons.Add(_elderIcon);
            return;
        }

        args.StatusIcons.Add(_ancientIcon);
    }
}
