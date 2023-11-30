using Robust.Shared.Prototypes;

namespace Content.Server.FriendlyFire;

[RegisterComponent]
public sealed partial class PrecisionShootingComponent : Component
{
    [DataField]
    public bool Enabled;

    [DataField]
    public EntProtoId Action = "ActionPrecisionShooting";

    [DataField]
    public EntityUid? ActionEntity;
}
