using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Overlays;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShowXenoAgeIconsComponent : Component
{
    [DataField, AutoNetworkedField]
    public long OverallRoleTime = 0;

    [DataField(required: true)]
    public ProtoId<PlayTimeTrackerPrototype> JobId;

}
