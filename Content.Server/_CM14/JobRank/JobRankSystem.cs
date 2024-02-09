using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Roles.Jobs;
using Content.Shared._CM14.JobRank;
using Robust.Server.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._CM14.JobRank;

public sealed class JobRankSystem : SharedJobRankSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;
    [Dependency] private readonly JobSystem _job = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = Logger.GetSawmill("job.rank");
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<JobRankComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            UpdateRank((uid, comp));
        }
    }

    public void UpdateRank(Entity<JobRankComponent> rank)
    {
        if (!TryGetRankPrototype(rank, out var protoId))
        {
            _sawmill.Fatal($"There is no {nameof(JobRankPrototype)} with the job {rank.Comp.Job}");
            return;
        }

        SetRank(rank, protoId);
    }

    private bool TryGetRankPrototype(Entity<JobRankComponent> rank, [NotNullWhen(true)] out JobRankPrototype? protoId)
    {
        var prototypes = _prototype.EnumeratePrototypes<JobRankPrototype>();
        foreach (var prototype in prototypes)
        {
            if (prototype.Job != rank.Comp.Job)
                continue;

            protoId = prototype;
            return true;
        }

        protoId = null;
        return false;
    }

    private void SetRank(Entity<JobRankComponent> rank, JobRankPrototype jobRank)
    {
        rank.Comp.Rank = jobRank;

        if (!_playerManager.TryGetSessionByEntity(rank, out var sessionId))
            return;

        foreach (var requirement in jobRank.Requirements.OrderBy(e => e.Time).Reverse())
        {
            if (_playTimeTracking.GetPlayTimeForTracker(sessionId, requirement.Role) < requirement.Time)
                continue;

            rank.Comp.Icon = requirement.Icon;
            Dirty(rank);
            break;
        }
    }
}
