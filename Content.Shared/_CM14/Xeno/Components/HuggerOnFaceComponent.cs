using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._CM14.Xeno.Components;

[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class HuggerOnFaceComponent : Component
{
    [AutoNetworkedField, DataField]
    public bool RootsCut = false;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan BurstAt;
}
