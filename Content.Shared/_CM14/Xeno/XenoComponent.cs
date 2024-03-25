using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xeno;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoComponent : Component
{
    [DataField]
    public List<EntProtoId> Actions = new() {"ActionXenoNightVision"};

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float HealthRegen = 0.25f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float HealthRegenOnRest = 3.5f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float HealthRegenOnWeeds = 9.75f;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public bool OnWeeds;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public bool OnResting;
}
