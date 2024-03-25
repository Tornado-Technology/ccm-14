using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared._CM14.Xeno;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoPsychicJumpSystem : EntitySystem
{
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoPsychicJumpComponent, XenoPsychicJumpEvent>(OnJump);
    }

    private void OnJump(EntityUid uid, XenoPsychicJumpComponent comp, XenoPsychicJumpEvent args)
    {
        var user = args.Performer;
        args.Handled = true;

        var origin = _transform.GetMapCoordinates(user);
        var target = args.Target.ToMap(EntityManager, _transform);

        // prevent collision with the user duh
        if (!_interaction.InRangeUnobstructed(origin, target, 0f, CollisionGroup.Opaque,
                entityUid => entityUid == user))
            return;

        _transform.SetCoordinates(user, args.Target);
        _transform.AttachToGridOrMap(user);
        // _audio.PlayPredicted(comp.BlinkSound, user, user);
    }
}
