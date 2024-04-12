using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._CM14.Xeno.Components;

[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class HuggerOnFaceComponent : Component
{
    [AutoNetworkedField, DataField]
    public bool RootsCut;

    [DataField]
    public float LayEggTime = 300f;

    [DataField]
    public float CurrentTime;

    [DataField("infectionEgg")]
    public string InfectionEgg = "MobEggedLarvaXeno";
}
