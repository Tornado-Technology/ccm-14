using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Xeno;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowXenoAgeSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShowXenoAgeIconsComponent, GetStatusIconsEvent>(OnGetStatusIcon);
    }

    private void OnGetStatusIcon(EntityUid uid, ShowXenoAgeIconsComponent component, ref GetStatusIconsEvent args)
    {
        if (!HasComp<XenoEvolutionsComponent>(uid))
            return;

        args.StatusIcons.Add(_prototype.Index<StatusIconPrototype>("XenoAgeIconAncient"));
    }
}
