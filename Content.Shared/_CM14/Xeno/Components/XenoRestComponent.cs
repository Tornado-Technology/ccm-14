using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xeno.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class XenoRestComponent : Component
{
    [DataField]
    public int? OriginalDrawDepth;

    public bool IsInRest = false;
}
