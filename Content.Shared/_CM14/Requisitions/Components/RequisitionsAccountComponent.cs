using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Requisitions.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class RequisitionsAccountComponent : Component
{
    [DataField]
    public bool Started;

    [DataField]
    public int Balance;

    [DataField]
    public int StartingDollarsPerMarine = 400;

    [DataField]
    public int GainPerMinute = 300;
}
