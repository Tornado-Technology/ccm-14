using Content.Server.Mind.Commands;
using Content.Shared.Mind;
using Robust.Shared.Containers;

namespace Content.Server._CM14.Xeno;

public sealed partial class XenoEggSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<XenoEggComponent>();
        while (query.MoveNext(out var uid, out var egg))
        {
            if (!_mindSystem.TryGetMind(uid, out var mindId, out var mind))
                continue;

            egg.CurHatchingTime += TimeSpan.FromSeconds(frameTime);

            if (egg.CurHatchingTime >= egg.HatchingTime)
                Hatch(uid, mindId, egg, mind);
        }
    }

    private void Hatch(EntityUid uid, EntityUid mindId, XenoEggComponent egg, MindComponent mind)
    {
        var targetTransformComp = Transform(uid);
        var child = Spawn(egg.Target, targetTransformComp.Coordinates);

        MakeSentientCommand.MakeSentient(child, EntityManager);

        var childXform = Transform(child);
        childXform.LocalRotation = targetTransformComp.LocalRotation;

        if (_containerSystem.TryGetContainingContainer(uid, out var cont))
        {
            _containerSystem.Insert(child, cont);
        }
            
        _mindSystem.TransferTo(mindId, child, mind: mind);
        EntityManager.DeleteEntity(uid);
    }
}
