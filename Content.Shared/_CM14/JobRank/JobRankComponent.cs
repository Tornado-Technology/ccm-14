using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.JobRank;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SharedJobRankSystem))]
public sealed partial class JobRankComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<JobPrototype>? Job;

    [DataField, AutoNetworkedField]
    public JobRankPrototype? Rank;

    [DataField, AutoNetworkedField]
    public ProtoId<StatusIconPrototype>? Icon;
}
