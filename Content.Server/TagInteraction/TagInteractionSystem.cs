using Content.Shared.Interaction.Events;
using Content.Shared.Tag;

namespace Content.Server.TagInteraction;

public sealed class TagInteractionSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TagInteractionComponent, InteractionAttemptEvent>(OnAttempt);
    }

    private void OnAttempt(Entity<TagInteractionComponent> ent, ref InteractionAttemptEvent args)
    {
        if (args.Target == null)
            return;

        if (ent.Comp.WhitelistTag != null && !_tag.HasTag((EntityUid) args.Target, ent.Comp.WhitelistTag))
            args.Cancel();

        if (ent.Comp.BlacklistTag != null && _tag.HasTag((EntityUid) args.Target, ent.Comp.BlacklistTag))
            args.Cancel();
    }
}
