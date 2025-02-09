using System.Linq;
using System.Numerics;
using Content.Shared._CM14.Xeno;
using Robust.Shared.Map;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoVinesSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoVinesComponent, XenoVinesEvent>(OnSpawnVines);
    }

    private void OnSpawnVines(EntityUid uid, XenoVinesComponent component, XenoVinesEvent args)
    {
        if (args.Handled)
            return;

        var transform = Transform(uid);

        if (transform.GridUid == null)
            return;

        var coords = transform.Coordinates;
        var result = false;

        // Spawn vines in center
        if (!IsTileBlockedByVines(coords))
        {
            Spawn(component.VinesPrototype, coords);
            result = true;
        }

        // Spawn vines in other directions
        for (var i = 0; i < 8; i++)
        {
            var direction = (Direction) i;
            var vector = direction.ToVec();
            coords = transform.Coordinates.Offset(new Vector2((float) Math.Round(vector.X),
                (float) Math.Round(vector.Y)));

            if (!IsTileBlockedByVines(coords))
            {
                Spawn(component.VinesPrototype, coords);
                result = true;
            }
        }

        if (result)
            args.Handled = true;
    }

    private bool IsTileBlockedByVines(EntityCoordinates coords)
    {
        return _lookupSystem.GetEntitiesInRange(coords, 0.001f).Any(HasComp<XenoWeedsComponent>);
    }
}
