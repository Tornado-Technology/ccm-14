﻿using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._CM14.Pulling;

public sealed class CMPullingSystem : EntitySystem
{
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly PullingSystem _pulling = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ParalyzeOnPullAttemptComponent, PullAttemptEvent>(OnParalyzeOnPullAttempt);

        SubscribeLocalEvent<SlowOnPullComponent, PullStartedMessage>(OnSlowPullStarted);
        SubscribeLocalEvent<SlowOnPullComponent, PullStoppedMessage>(OnSlowPullStopped);

        SubscribeLocalEvent<PullingSlowedComponent, RefreshMovementSpeedModifiersEvent>(OnPullingSlowedMovementSpeed);

        SubscribeLocalEvent<PullWhitelistComponent, StartPullAttemptEvent>(OnPullWhitelistStartPullAttempt);

        SubscribeLocalEvent<BlockPullingDeadComponent, StartPullAttemptEvent>(OnBlockDeadStartPullAttempt);
        SubscribeLocalEvent<BlockPullingDeadComponent, PullStartedMessage>(OnBlockDeadPullStarted);
        SubscribeLocalEvent<BlockPullingDeadComponent, PullStoppedMessage>(OnBlockDeadPullStopped);
    }

    private void OnParalyzeOnPullAttempt(Entity<ParalyzeOnPullAttemptComponent> ent, ref PullAttemptEvent args)
    {
        var user = args.PullerUid;
        var target = args.PulledUid;
        if (target != ent.Owner ||
            HasComp<ParalyzeOnPullAttemptImmuneComponent>(user) ||
            _mobState.IsIncapacitated(ent))
        {
            return;
        }

        _stun.TryParalyze(user, ent.Comp.Duration, true);
        args.Cancelled = true;

        if (!_timing.IsFirstTimePredicted)
            return;

        foreach (var session in Filter.Pvs(user).Recipients)
        {
            if (session == IoCManager.Resolve<ISharedPlayerManager>().LocalSession)
                continue;

            var puller = Identity.Name(user, EntityManager, session.AttachedEntity);
            var pulled = Identity.Name(ent, EntityManager, session.AttachedEntity);
            var message = $"{puller} tried to pull {pulled} but instead gets a tail swipe to the head!";
            _popup.PopupEntity(message, user, session, PopupType.MediumCaution);
        }
    }

    private void OnSlowPullStarted(Entity<SlowOnPullComponent> ent, ref PullStartedMessage args)
    {
        if (ent.Owner == args.PulledUid)
        {
            EnsureComp<PullingSlowedComponent>(args.PullerUid);
            _movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
        }
    }

    private void OnSlowPullStopped(Entity<SlowOnPullComponent> ent, ref PullStoppedMessage args)
    {
        if (ent.Owner == args.PulledUid)
        {
            RemCompDeferred<PullingSlowedComponent>(args.PullerUid);
            _movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
        }
    }

    private void OnPullingSlowedMovementSpeed(Entity<PullingSlowedComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (TryComp(ent, out PullerComponent? puller) &&
            TryComp(puller.Pulling, out SlowOnPullComponent? slow))
        {
            args.ModifySpeed(slow.Multiplier, slow.Multiplier);
        }
    }

    private void OnPullWhitelistStartPullAttempt(Entity<PullWhitelistComponent> ent, ref StartPullAttemptEvent args)
    {
        if (args.Cancelled || ent.Owner == args.Pulled)
            return;

        if (!_whitelist.IsValid(ent.Comp.Whitelist, args.Pulled))
        {
            var name = Loc.GetString("zzzz-the", ("ent", args.Pulled));
            _popup.PopupClient($"We have no use for {name}, why would we want to touch it?", args.Pulled, args.Puller);
            args.Cancel();
        }
    }

    private void OnBlockDeadStartPullAttempt(Entity<BlockPullingDeadComponent> ent, ref StartPullAttemptEvent args)
    {
        if (args.Cancelled || ent.Owner == args.Pulled)
            return;

        if (_mobState.IsDead(args.Pulled))
        {
            var name = Loc.GetString("zzzz-the", ("ent", args.Pulled));
            _popup.PopupClient($"{name} is dead, why would we want to touch it?", args.Pulled, args.Puller);
            args.Cancel();
        }
    }

    private void OnBlockDeadPullStarted(Entity<BlockPullingDeadComponent> ent, ref PullStartedMessage args)
    {
        if (ent.Owner == args.PullerUid)
            EnsureComp<BlockPullingDeadActiveComponent>(ent);
    }

    private void OnBlockDeadPullStopped(Entity<BlockPullingDeadComponent> ent, ref PullStoppedMessage args)
    {
        if (ent.Owner == args.PullerUid)
            RemCompDeferred<BlockPullingDeadActiveComponent>(ent);
    }

    public override void Update(float frameTime)
    {
        var blockDeadActive = EntityQueryEnumerator<BlockPullingDeadActiveComponent, PullerComponent>();
        while (blockDeadActive.MoveNext(out var uid, out _, out var puller))
        {
            if (puller.Pulling is not { } pulling ||
                !TryComp(pulling, out PullableComponent? pullable))
            {
                continue;
            }

            if (_mobState.IsDead(pulling))
                _pulling.TryStopPull(pulling, pullable, uid);
        }
    }
}
