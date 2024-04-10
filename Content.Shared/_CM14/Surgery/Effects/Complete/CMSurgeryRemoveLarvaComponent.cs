using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Effects.Complete;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMSurgeryRemoveLarvaComponent : Component
{
}
