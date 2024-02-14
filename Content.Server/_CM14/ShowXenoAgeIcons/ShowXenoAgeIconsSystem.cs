using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Mind.Components;
using Content.Shared.Overlays;
using Robust.Server.Player;

namespace Content.Server._CM14.ShowXenoAgeIcons;

public sealed class ShowXenoAgeIconsSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<ShowXenoAgeIconsComponent>();
        while (query.MoveNext(out var uid, out var showXenoAgeIcons))
        {
            showXenoAgeIcons.OverallRoleTime = 0;
            if (!_entManager.TryGetComponent<MindContainerComponent>(uid, out var mindContainer))
                continue;

            if (!mindContainer.HasMind)
                continue;

            if (!_playerManager.TryGetSessionByEntity(uid, out var sessionId))
                continue;

            showXenoAgeIcons.OverallRoleTime = _playTimeTracking.GetPlayTimeForTracker(sessionId, showXenoAgeIcons.JobId).Seconds;
            Dirty(uid, showXenoAgeIcons);
        }
    }
}
