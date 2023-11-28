using Content.Shared.Xeno.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Xeno.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(XenoEvasionSystem))]
public sealed partial class XenoEvadesComponent : Component
{

}
