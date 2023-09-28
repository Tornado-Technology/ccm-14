using Content.Shared.Damage;

namespace Content.Server.Alien;


[RegisterComponent]
public sealed partial class BrainSlugComponent : Component
{

    public EntityUid EquipedOn;

    public EntityUid OwnerId;
    public bool isEgged;
    public bool isDeath;

    [DataField("infectionEgg")]
    public string InfectionEgg = "FoodEgg";//XenoEgg

    [DataField("damageFrequency"), ViewVariables(VVAccess.ReadWrite)]
    public float DamageFrequency = 1;

    [ViewVariables] public float Accumulator = 0;

    [DataField("infectionFrequency"), ViewVariables(VVAccess.ReadWrite)]
    public float InfectionFrequency = 10;//60

    [ViewVariables] public float InfectionAccumulator = 0;

    [DataField("paralyzeTime"), ViewVariables(VVAccess.ReadWrite)]
    public float ParalyzeTime = 3f;

    //[DataField("damage", required: true)]
    //[ViewVariables(VVAccess.ReadWrite)]
    //public DamageSpecifier Damage = default!;

    public bool IsDeath = false;
}
