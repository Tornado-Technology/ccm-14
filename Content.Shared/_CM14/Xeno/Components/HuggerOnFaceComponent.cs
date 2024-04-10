using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._CM14.Xeno.Components;

[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class HuggerOnFaceComponent : Component
{
    [AutoNetworkedField, DataField]
    public bool RootsCut = false;

    [DataField]
    public float LayEggTime = 480f;

    [DataField]
    public float CurrentTime = 0f;

    [DataField("infectionEgg")]
    public string InfectionEgg = "XenoLarvaEgg";
}
