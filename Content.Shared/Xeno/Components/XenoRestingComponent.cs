using Content.Shared.Xeno.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Xeno.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(XenoRestSystem))]
public sealed partial class XenoRestingComponent : Component
{

}
