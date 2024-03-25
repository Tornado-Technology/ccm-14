using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xenos.Acid;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(XenoAcidSystem))]
public sealed partial class XenoAcidComponent : Component
{
    [DataField]
    public EntProtoId AcidId = "XenoAcid";

    [DataField]
    public TimeSpan AcidTime = TimeSpan.FromSeconds(30);

    [DataField, AutoNetworkedField]
    public TimeSpan AcidDelay = TimeSpan.FromSeconds(5);
}
