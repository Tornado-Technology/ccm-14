using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Surgery.Steps;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMSurgeryStepComponent : Component
{
    [DataField, AutoNetworkedField]
    public int MinSkill = 1;

    [DataField, AutoNetworkedField]
    public int GoodSkill = 1;

    [DataField, AutoNetworkedField]
    public float DoAfter = 2.5f;

    [DataField]
    public ComponentRegistry? Tool;

    [DataField]
    public ComponentRegistry? Add;

    [DataField]
    public ComponentRegistry? Remove;

    [DataField]
    public ComponentRegistry? BodyRemove;
}
