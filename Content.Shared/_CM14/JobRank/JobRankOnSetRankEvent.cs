namespace Content.Shared._CM14.JobRank;

public sealed class JobRankOnSetRankEvent : EntityEventArgs
{
    public readonly JobRankPrototype JobRank;

    public JobRankOnSetRankEvent(JobRankPrototype jobRank)
    {
        JobRank = jobRank;
    }
}
