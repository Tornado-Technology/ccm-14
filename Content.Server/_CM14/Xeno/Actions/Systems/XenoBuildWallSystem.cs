using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Content.Server._CM14.Xeno.Actions.Components;
using Content.Shared._CM14.Xeno;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoBuildWallSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ISharedAdminLogManager _logs = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoBuildWallComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoBuildWallComponent, XenoBuildWallEvent>(OnBuild);
        SubscribeLocalEvent<XenoBuildWallComponent, XenoBuildWallDoAfterEvent>(OnBuildDoAfter);
    }

    private void OnStartup(EntityUid uid, XenoBuildWallComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnBuild(Entity<XenoBuildWallComponent> ent, ref XenoBuildWallEvent args)
    {
        if (!CanBuildOnTilePopup(ent, args.Target))
            return;

        var doAfterEventArgs =
           new DoAfterArgs(EntityManager, ent, TimeSpan.FromSeconds(ent.Comp.TimeUsage), new XenoBuildWallDoAfterEvent(args.Target.ToMap(EntityManager, _transform)), ent, target: ent, used: ent)
           {
               BreakOnUserMove = true,
               BreakOnTargetMove = false,
               NeedHand = false,
               BreakOnDamage = true,
               BlockDuplicate = false,
               CancelDuplicate = false,
           };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void OnBuildDoAfter(Entity<XenoBuildWallComponent> ent, ref XenoBuildWallDoAfterEvent args)
    {
        args.Repeat = false;
        if (args.Handled || args.Cancelled || !CanBuildOnTilePopup(ent, new EntityCoordinates(_map.GetMapEntityId(args.Coordinates.MapId), args.Coordinates.Position)))
            return;

        var wallUid = Spawn(ent.Comp.WallPrototype, args.Coordinates);

        _logs.Add(LogType.Construction,
            $"{ToPrettyString(ent):user} was build xeno wall {ToPrettyString(wallUid)}");
    }

    private bool TileSolidAndNotBlocked(EntityCoordinates target)
    {
        return target.GetTileRef(EntityManager, _map) is { } tile &&
               !tile.IsSpace() &&
               tile.GetContentTileDefinition().Sturdy &&
               !_turf.IsTileBlocked(tile, CollisionGroup.Impassable);
    }

    private bool InRangePopup(EntityUid xeno, EntityCoordinates target, float range)
    {
        var origin = _transform.GetMoverCoordinates(xeno);
        if (origin.InRange(EntityManager, _transform, target, range))
            return true;

        _popup.PopupClient(Loc.GetString("xeno-cant-reach-there"), xeno, xeno);
        return false;
    }

    private bool CanBuildOnTilePopup(Entity<XenoBuildWallComponent> xeno, EntityCoordinates target)
    {
        // TODO (CM): calculate range limit from grid-snapped coordinates
        if (InRangePopup(xeno, target, 2f) && TileSolidAndNotBlocked(target))
            return true;

        _popup.PopupClient(Loc.GetString("xeno-cant-reach-there"), xeno, xeno);
        return false;
    }
}
