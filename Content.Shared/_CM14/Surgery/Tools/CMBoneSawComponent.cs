using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Tools;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMBoneSawComponent : Component, ICMSurgeryToolComponent
{
    public string ToolName => "a bone saw";
}
