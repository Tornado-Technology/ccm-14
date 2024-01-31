using Content.Server.Mech.Systems;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Gravity;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Timing;
using System.Linq;
using Robust.Shared.Map;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Components;
using System.Linq;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Wall;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Content.Server.Medical.Components;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Climbing.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Medical.Cryogenics;
using Robust.Shared.Timing;
using SharedToolSystem = Content.Shared.Tools.Systems.SharedToolSystem;

namespace Content.Server.Mech.Equipment.EntitySystems;
public sealed class MechGunSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly MechSystem _mech = default!;
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly SharedGunSystem _guns = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MechEquipmentComponent, GunShotEvent>(MechGunShot);
    }

    private void MechGunShot(EntityUid uid, MechEquipmentComponent component, ref GunShotEvent args)
    {
        if (!component.EquipmentOwner.HasValue)
        {
            _stun.TryParalyze(args.User, TimeSpan.FromSeconds(10), true);
            _throwing.TryThrow(args.User, _random.NextVector2(), _random.Next(500));
            return;
        }
        if (!TryComp<MechComponent>(component.EquipmentOwner.Value, out var mech))
        {
            _stun.TryParalyze(args.User, TimeSpan.FromSeconds(10), true);
            _throwing.TryThrow(args.User, _random.NextVector2(), _random.Next(50));
            return;
        }
        if (TryComp<BatteryComponent>(uid, out var battery))
        {
            ChargeGunBattery(uid, battery);
            return;
        }
        // In most guns the ammo itself isn't shot but turned into cassings
        // and a new projectile is spawned instead, meaning that args.Ammo
        // is most likely inside the equipment container (for some odd reason)

        // I'm not even sure why this is needed since GunSystem.Shoot() has a
        // container check before ejecting, but yet it still puts the spent ammo inside the mech
        foreach (var (ent, _) in args.Ammo)
        {
            if (ent.HasValue && mech.EquipmentContainer.Contains(ent.Value))
            {
                mech.EquipmentContainer.Remove(ent.Value);
                _throwing.TryThrow(ent.Value, _random.NextVector2(), _random.Next(5));
            }
        }
    }

    private void ChargeGunBattery(EntityUid uid, BatteryComponent component)
    {
        if (!TryComp<MechEquipmentComponent>(uid, out var mechEquipment) || !mechEquipment.EquipmentOwner.HasValue)
            return;

        if (!TryComp<MechComponent>(mechEquipment.EquipmentOwner.Value, out var mech))
            return;

        var maxCharge = component.MaxCharge;
        var currentCharge = component.CurrentCharge;

        var chargeDelta = maxCharge - currentCharge;

        if (chargeDelta <= 0 || mech.Energy - chargeDelta < 0)
            return;

        if (!_mech.TryChangeEnergy(mechEquipment.EquipmentOwner.Value, -chargeDelta, mech))
            return;

        _battery.SetCharge(uid, component.MaxCharge, component);
    }
}
