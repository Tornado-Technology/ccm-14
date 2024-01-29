using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xeno;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoComponent : Component
{
    [DataField]
    public EntProtoId ActionNightVision = "ActionXenoNightVision";

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
