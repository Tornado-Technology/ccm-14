using Content.Server.Spawners.Components;
using Robust.Shared.Random;
using System.Linq;

namespace Content.Server.Spawners.EntitySystems;

public sealed class RandomPointSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RandomPointSpawnerComponent, MapInitEvent>(OnSpawnMapInit);
    }

    private void OnSpawnMapInit(EntityUid uid, RandomPointSpawnerComponent component, MapInitEvent args)
    {
        if (component.Spawned)
            return;

        var spawners = EntityQuery<RandomPointSpawnerComponent>().Where((comp) => comp.GroupId == component.GroupId).ToList();

        if (spawners.Count == 0)
            return;

        foreach (var spawner in spawners)
        {
            component.Spawned = true;
        }

        var random = _random.Next(0, spawners.Count - 1);
        var targetSpawner = spawners[random];

        if (targetSpawner == null)
            return;

        var coords = Transform(targetSpawner.Owner).Coordinates;
        var prototypes = targetSpawner.Prototypes;

        if (prototypes.Count == 0)
        {
            Log.Warning($"Prototype list in RandomPointSpawnerComponent is empty! Entity: {ToPrettyString(targetSpawner.Owner)}");
            return;
        }

        foreach (var proto in prototypes)
        {
            Spawn(proto, coords);
        }
    }
}
