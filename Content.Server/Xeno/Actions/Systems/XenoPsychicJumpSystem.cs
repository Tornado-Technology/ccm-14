using Content.Server.Xeno.Actions.Components;
using Content.Shared.Actions;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Xeno;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server.Xeno.Actions.Systems;

public sealed class XenoPsychicJumpSystem : EntitySystem
{
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoPsychicJumpComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoPsychicJumpComponent, XenoPsychicJumpEvent>(OnJump);
    }

    private void OnStartup(EntityUid uid, XenoPsychicJumpComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnJump(EntityUid uid, XenoPsychicJumpComponent comp, XenoPsychicJumpEvent args)
    {
        var user = args.Performer;
        args.Handled = true;

        var origin = Transform(user).MapPosition;
        var target = args.Target.ToMap(EntityManager, _transform);

        // prevent collision with the user duh
        if (!_interaction.InRangeUnobstructed(origin, target, 0f, CollisionGroup.Opaque, uid => uid == user))
            return;

        _transform.SetCoordinates(user, args.Target);
        _transform.AttachToGridOrMap(user);
        // _audio.PlayPredicted(comp.BlinkSound, user, user);
    }
}
