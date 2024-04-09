using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Steps.Parts;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMIncisionOpenComponent : Component
{
}
