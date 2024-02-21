using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xeno.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class XenoEvasionComponent : Component
{
    [DataField]
    public EntProtoId Action = "ActionXenoEvasion";

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Duration = 2.5f;

    [DataField]
    public float DurationTime;
}
