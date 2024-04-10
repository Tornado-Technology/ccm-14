using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Conditions;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMSurgeryLarvaConditionComponent : Component
{
}
