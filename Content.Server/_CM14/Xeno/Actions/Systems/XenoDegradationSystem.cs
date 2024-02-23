using Content.Server.Body.Systems;
using Content.Server.Mind.Commands;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Content.Server._CM14.Xeno.Actions.Components;
using Content.Shared._CM14.Xeno;


namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoDegradationSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    [Dependency] private readonly SharedContainerSystem _container = default!;

    [Dependency] private readonly SharedMindSystem _mind = default!;

    [Dependency] private readonly PopupSystem _popupSystem = default!;

    [Dependency] private readonly SharedAudioSystem _audio = default!;

    [Dependency] private readonly BodySystem _bodySystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoDegradationComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<XenoDegradationComponent, XenoDegradationEvent>(OnDegradation);
        SubscribeLocalEvent<XenoDegradationComponent, XenoDegradationDoAfterEvent>(OnDegradationDoAfter);
    }

    private void OnStartup(EntityUid uid, XenoDegradationComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.Action);
    }

    private void OnDegradation(Entity<XenoDegradationComponent> ent, ref XenoDegradationEvent args)
    {
        var doAfterEventArgs = new DoAfterArgs(EntityManager, ent, TimeSpan.FromSeconds(ent.Comp.TimeUsage),
            new XenoDegradationDoAfterEvent(), ent, target: ent, used: ent)
        {
            BreakOnUserMove = true,
            BreakOnTargetMove = false,
            NeedHand = false,
            BreakOnDamage = true,
            BlockDuplicate = true,
            CancelDuplicate = false,
        };
        _popupSystem.PopupEntity(Loc.GetString("comp-xeno-degradation", ("xeno", ent.Owner)), ent,
            PopupType.LargeCaution);
        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void OnDegradationDoAfter(Entity<XenoDegradationComponent> ent, ref XenoDegradationDoAfterEvent args)
    {
        args.Repeat = false;
        if (args.Handled || args.Cancelled)
        {
            return;
        }

        var targetTransformComp = Transform(ent);
        var child = Spawn(ent.Comp.Prototype, targetTransformComp.Coordinates);

        MakeSentientCommand.MakeSentient(child, EntityManager);

        var childXform = Transform(child);
        childXform.LocalRotation = targetTransformComp.LocalRotation;

        if (_container.TryGetContainingContainer(ent, out var cont))
        {
            _container.Insert(child, cont);
        }

        if (_mind.TryGetMind(ent, out var mindId, out var mind))
            _mind.TransferTo(mindId, child, mind: mind);
        var gibs = _bodySystem.GibBody(ent);
        foreach (var gib in gibs)
        {
            QueueDel(gib);
        }

        _audio.PlayEntity(ent.Comp.DegradationSound, Filter.Pvs(child), child, true);
        EntityManager.DeleteEntity(ent);
    }
}
