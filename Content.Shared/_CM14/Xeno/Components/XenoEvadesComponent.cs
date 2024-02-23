using Content.Shared._CM14.Xeno.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Xeno.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(XenoEvasionSystem))]
public sealed partial class XenoEvadesComponent : Component
{

}
