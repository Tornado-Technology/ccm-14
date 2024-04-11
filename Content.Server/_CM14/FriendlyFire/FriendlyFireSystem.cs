using System.Linq;
using Content.Server.NPC.Components;
using Content.Shared.Projectiles;
using Content.Shared._CM14.FriendlyFire;
using Robust.Shared.GameStates;
using Content.Shared.NPC.Components;

namespace Content.Server._CM14.FriendlyFire;

public sealed partial class FriendlyFireSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FriendlyFireComponent, ProjectileHitAttemptEvent>(OnHitAttempt);

        ToggleableInitialize();
    }

    private void OnHitAttempt(Entity<FriendlyFireComponent> ent, ref ProjectileHitAttemptEvent args)
    {
        if (args.Shooter == null)
            return;

        if (!TryComp<FriendlyFireComponent>(args.Shooter, out var friendlyFireShooter))
            return;

        if (!TryComp<NpcFactionMemberComponent>(ent, out var faction) || !TryComp<NpcFactionMemberComponent>(args.Shooter, out var factionShooter))
            return;

        if (faction.Factions.Count == 0 || factionShooter.Factions.Count == 0)
            return;

        if (faction.Factions.Any(x => factionShooter.Factions.Any(y => y == x)) && !friendlyFireShooter.Enabled)
            args.Cancel();
    }

    public void Toggle(EntityUid ent)
    {
        if (!TryComp<FriendlyFireComponent>(ent, out var friendlyFire))
            return;

        friendlyFire.Enabled = !friendlyFire.Enabled;

        Dirty(ent, friendlyFire);
    }

    public void SetEnabled(EntityUid ent, bool state)
    {
        if (!TryComp<FriendlyFireComponent>(ent, out var friendlyFire))
            return;

        friendlyFire.Enabled = state;
        Dirty(ent, friendlyFire);
    }

    public bool GetEnabled(EntityUid ent)
    {
        return TryComp<FriendlyFireComponent>(ent, out var friendlyFire) && friendlyFire.Enabled;
    }
}
