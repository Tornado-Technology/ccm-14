using System.Linq;
using Content.Server.NPC.Components;
using Content.Shared.Projectiles;

namespace Content.Server.FriendlyFire;

public sealed class FriendlyFireSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FriendlyFireComponent, ProjectileHitAttemptEvent>(OnHitAttempt);
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

    public void SetEnabled(EntityUid ent, bool state)
    {
        if (!TryComp<FriendlyFireComponent>(ent, out var friendlyFire))
            return;

        friendlyFire.Enabled = state;
    }

    public bool GetEnabled(EntityUid ent)
    {
        return TryComp<FriendlyFireComponent>(ent, out var friendlyFire) && friendlyFire.Enabled;
    }
}
