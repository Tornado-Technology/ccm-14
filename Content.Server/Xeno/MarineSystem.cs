using Content.Server.Xeno.Components;
using Content.Shared.Roles.Jobs;
using Robust.Server.GameObjects;

namespace Content.Server.Xeno;

public sealed class MarineSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MarineComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, MarineComponent component, ComponentStartup args)
    {
        if (!TryComp<ActorComponent>(uid, out var actor))
            return;

        if (!TryComp<JobComponent>(uid, out var job))
            return;
    }
}
