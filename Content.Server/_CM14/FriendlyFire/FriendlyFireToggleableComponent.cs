using Robust.Shared.Prototypes;

namespace Content.Server._CM14.FriendlyFire;

[RegisterComponent]
public sealed partial class FriendlyFireToggleableComponent : Component
{
    [DataField]
    public EntProtoId Prototype = "ActionPrecisionShooting";

    [DataField]
    public EntityUid? Action;

    [DataField]
    public float FireRateModifier = 0.2f;
}
