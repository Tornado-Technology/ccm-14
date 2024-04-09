using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Tools;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedCMSurgerySystem))]
public sealed partial class CMBoneGelComponent : Component, ICMSurgeryToolComponent
{
    public string ToolName => "bone gel";
}
