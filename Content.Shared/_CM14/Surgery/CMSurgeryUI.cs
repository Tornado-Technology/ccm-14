using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.Surgery;

[Serializable, NetSerializable]
public enum CMSurgeryUIKey
{
    Key
}

[Serializable, NetSerializable]
public sealed class CMSurgeryBuiState : BoundUserInterfaceState
{
    public readonly Dictionary<NetEntity, List<EntProtoId>> Choices;

    public CMSurgeryBuiState(Dictionary<NetEntity, List<EntProtoId>> choices)
    {
        Choices = choices;
    }
}

[Serializable, NetSerializable]
public sealed class CMSurgeryStepChosenBuiMessage : BoundUserInterfaceMessage
{
    public readonly NetEntity Part;
    public readonly EntProtoId Surgery;
    public readonly EntProtoId Step;

    public CMSurgeryStepChosenBuiMessage(NetEntity part, EntProtoId surgery, EntProtoId step)
    {
        Part = part;
        Surgery = surgery;
        Step = step;
    }
}
