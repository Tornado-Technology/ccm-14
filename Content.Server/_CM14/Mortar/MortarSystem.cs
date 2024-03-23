using System.Numerics;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Popups;
using Content.Server.Projectiles;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Server.Audio;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;

namespace Content.Server._CM14.Mortar;

public sealed partial class MortarSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly ProjectileSystem _projectile = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeCommands();
        InitializeUI();
    }

    private void TryLaunch(Entity<MortarComponent> mortar, Vector2 position, EntityUid? user = null)
    {
        TryLaunch(mortar, new MapCoordinates(position, Transform(mortar).MapID), user);
    }

    private void TryLaunch(Entity<MortarComponent> mortar, MapCoordinates coordinates, EntityUid? user = null)
    {
        if (GetShell(mortar) is not { } shell)
        {
            _popup.PopupEntity("", mortar);
            return;
        }

        if (!TryComp<CartridgeAmmoComponent>(shell, out var cartridgeAmmo))
        {
            _popup.PopupEntity("", mortar);
            return;
        }

        if (cartridgeAmmo.Spent)
        {
            _popup.PopupEntity("", mortar);
            return;
        }

        for (var i = 0; i < cartridgeAmmo.Count; i++)
        {
            // Create a projectile
            var bullet = Spawn(cartridgeAmmo.Prototype, coordinates);

            // We make the projectile hang in the air
            var physics = EnsureComp<PhysicsComponent>(bullet);
            _physics.SetBodyStatus(physics, BodyStatus.InAir);
            _physics.SetLinearVelocity(bullet, Vector2.Zero, body: physics);

            // Mainly we need it for logs, lol
            var projectile = EnsureComp<ProjectileComponent>(bullet);
            _projectile.SetShooter(bullet, projectile, user ?? mortar);
            projectile.Weapon = mortar;

            // Let's blow it up immediately
            _trigger.Trigger(bullet);

            // BOOM!
            _audio.PlayPvs(mortar.Comp.LaunchSound.CanonPath, Transform(mortar).Coordinates);
        }

        // Clean the cartridge if necessary
        if (cartridgeAmmo.DeleteOnSpawn)
        {
            Del(shell);
            return;
        }

        // Making it spent if we don't need to delete it
        cartridgeAmmo.Spent = true;
        Dirty(shell, cartridgeAmmo);
    }

    private EntityUid? GetShell(Entity<MortarComponent> mortar)
    {
        var slot = _container.EnsureContainer<ContainerSlot>(mortar, mortar.Comp.ShellContainerId);
        return slot.ContainedEntity;
    }
}
