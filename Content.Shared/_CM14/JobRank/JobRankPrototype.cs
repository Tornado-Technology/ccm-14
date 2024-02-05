using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.JobRank;

[Serializable, Prototype("jobRank")]
public sealed class JobRankPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public ProtoId<JobPrototype> Job;

    [DataField]
    public HashSet<JobRankTimeRequirement> Requirements = new();
}

[Serializable, NetSerializable, ImplicitDataDefinitionForInheritors]
public partial class JobRankTimeRequirement
{
    [DataField]
    public ProtoId<PlayTimeTrackerPrototype> Role;

    [DataField]
    public ProtoId<StatusIconPrototype> Icon;

    [DataField]
    public TimeSpan Time;
}

