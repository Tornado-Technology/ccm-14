using Content.Server.NPC.Components;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Hands;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Prototypes;
using Content.Server._CM14.Xeno.Mobs.Components;
using Content.Shared._CM14.Xeno;
using Robust.Server.GameObjects;
using Content.Shared._CM14.Xeno.Components;

namespace Content.Server._CM14.Xeno.Mobs.Systems;

public sealed class FaceHuggerSystem : SharedFaceHuggingSystem
{
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedCombatModeSystem _combat = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<FaceHuggingComponent, ComponentStartup>(OnStartup);

        SubscribeLocalEvent<FaceHuggingComponent, FaceHuggerJumpActionEvent>(OnJumpFaceHugger);
        SubscribeLocalEvent<FaceHuggerComponent, ThrowDoHitEvent>(OnFaceHuggerDoHit);

        SubscribeLocalEvent<FaceHuggerComponent, GotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<FaceHuggerComponent, BeingUnequippedAttemptEvent>(OnUnequipAttempt);
        SubscribeLocalEvent<FaceHuggerComponent, GotEquippedHandEvent>(OnGotEquippedHand);
        SubscribeLocalEvent<FaceHuggerComponent, GotUnequippedEvent>(OnGotUnequipped);
        SubscribeLocalEvent<FaceHuggerComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnStartup(EntityUid uid, FaceHuggingComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, component.FaceHuggerJumpAction);
    }

    private void OnFaceHuggerDoHit(EntityUid uid, FaceHuggerComponent component, ThrowDoHitEvent args)
    {
        if (component.IsDeath)
            return;

        if (TryComp(args.Target, out HuggerOnFaceComponent? _))
            return;

        if (HasComp<FaceHuggerComponent>(args.Target))
            return;

        if (HasComp<XenoComponent>(args.Target))
            return;


        TryComp(uid, out FaceHuggerComponent? faceHuggerComponent);
        if (faceHuggerComponent == null)
            return;

        if (!HasComp<HumanoidAppearanceComponent>(args.Target))
            return;

        if (!TryComp(args.Target, out MobStateComponent? mobState))
            return;

        if (mobState.CurrentState != MobState.Alive && mobState.CurrentState != MobState.Critical)
            return;

        if (!TryComp(args.Target, out InventoryComponent? inventory))
            return;


        _entityManager.AddComponent<HuggerOnFaceComponent>(args.Target);
        _inventory.TryUnequip(args.Target, "mask", true, true, false, inventory);
        var equipped = _inventory.TryEquip(args.Target, uid, "mask", true, true, false, inventory);
        if (!equipped)
            return;

        component.Equipped = args.Target;

        RemComp<CombatModeComponent>(uid);

        _stunSystem.TryParalyze(args.Target, TimeSpan.FromSeconds(component.ParalyzeTime), true);

        faceHuggerComponent.Equipped = args.Target;

        _popup.PopupEntity(Loc.GetString("Something jumped on you!"), args.Target, args.Target, PopupType.LargeCaution);
    }

    private void OnJumpFaceHugger(EntityUid uid, FaceHuggingComponent component, FaceHuggerJumpActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;
        var xform = Transform(uid);
        var mapCoords = args.Target.ToMap(EntityManager, _transform);
        var direction = mapCoords.Position - _transform.GetMapCoordinates(xform).Position;

        _throwing.TryThrow(uid, direction, 7F, uid, 10F);
    }

    private void OnGotEquipped(EntityUid uid, FaceHuggerComponent component, GotEquippedEvent args)
    {
        if (args.Slot != "mask")
            return;

        component.Equipped = args.Equipee;

        RemComp<CombatModeComponent>(uid);

        if (TryComp<XenoEvolutionsComponent>(uid, out var evolution))
            evolution.Enabled = false;
    }

    private void OnUnequipAttempt(EntityUid uid, FaceHuggerComponent component, BeingUnequippedAttemptEvent args)
    {
        if (args.Slot != "mask")
            return;

        if (component.Equipped != args.Unequipee)
            return;

        if (HasComp<FaceHuggerComponent>(args.Unequipee))
            return;
        args.Cancel();
    }

    private void OnGotEquippedHand(EntityUid uid, FaceHuggerComponent component, GotEquippedHandEvent args)
    {
        if (component.IsDeath)
            return;

        _damageableSystem.TryChangeDamage(args.User,
            new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 5));
    }

    private void OnGotUnequipped(EntityUid uid, FaceHuggerComponent component, GotUnequippedEvent args)
    {
        if (args.Slot != "mask")
            return;

        component.Equipped = new EntityUid();
        var combatMode = EntityManager.AddComponent<CombatModeComponent>(uid);

        _combat.SetInCombatMode(uid, true, combatMode);

        if (TryComp<XenoEvolutionsComponent>(uid, out var evolution))
            evolution.Enabled = true;

        EnsureComp<NPCMeleeCombatComponent>(uid);
    }

    private static void OnMobStateChanged(EntityUid uid, FaceHuggerComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead)
        {
            component.IsDeath = true;
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var comp in EntityQuery<FaceHuggerComponent>())
        {
            if (comp.Equipped is not { Valid: true } targetId)
            {
                continue;
            }

            if (TryComp(targetId, out MobStateComponent? mobState))
            {
                if (mobState.CurrentState is MobState.Dead)
                {
                    _inventory.TryUnequip(targetId, "mask", true, true);
                    EntityManager.RemoveComponent<HuggerOnFaceComponent>(targetId);
                    comp.Equipped = new EntityUid();
                    return;
                }
            }

            if (!HasComp<HuggerOnFaceComponent>(targetId))
                return;

            if (TryComp(targetId, out HuggerOnFaceComponent? huggerOnFaceComponent))
            {
                huggerOnFaceComponent.CurrentTime += frameTime;
            }
        }

        var query = EntityQueryEnumerator<HuggerOnFaceComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.RootsCut)
            {
                comp.CurrentTime += frameTime / 3;
            }
            else
            {
                comp.CurrentTime += frameTime;
            }
            if (comp.CurrentTime >= comp.LayEggTime)
            {
                _inventory.TryUnequip(uid, "mask", true, true);

                Spawn(comp.InfectionEgg, Transform(uid).Coordinates);
                _damageableSystem.TryChangeDamage(uid,
                    new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), 10000));
            }
        }
    }
}
