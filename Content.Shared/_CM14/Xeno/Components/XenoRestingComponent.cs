using Content.Shared._CM14.Xeno.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Xeno.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(XenoRestSystem))]
public sealed partial class XenoRestingComponent : Component
{

}
