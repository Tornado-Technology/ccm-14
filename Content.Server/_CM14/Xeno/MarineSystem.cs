using Content.Server.Chat.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Player;

namespace Content.Server._CM14.Xeno;

public sealed class MarineSystem : EntitySystem
{
    [Dependency] private readonly AccessReaderSystem _accessReader = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MarineComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<MarineComponent, TransformSpeakerNameEvent>(OnTransformSpeakerName);
    }

    private void OnTransformSpeakerName(EntityUid _, MarineComponent component, TransformSpeakerNameEvent args)
    {
        var jobName = string.Empty;
        if (_accessReader.FindAccessItemsInventory(args.Sender, out var items))
        {
            foreach (var item in items)
            {
                // ID Card
                if (TryComp(item, out IdCardComponent? id))
                {
                    jobName = id.JobTitle;
                    break;
                }
            }
        }

        if (jobName != string.Empty)
        {
            args.Name = $"[{jobName}] {Name(args.Sender)}";
        }
        else
        {
            args.Name = "~" + Name(args.Sender);
        }
    }

    private void OnStartup(EntityUid uid, MarineComponent component, ComponentStartup args)
    {
        if (!TryComp<ActorComponent>(uid, out var actor))
            return;

        if (!TryComp<JobComponent>(uid, out var job))
            return;
    }
}
