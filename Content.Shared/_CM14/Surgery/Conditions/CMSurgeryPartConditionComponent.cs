using Content.Shared.Body.Part;
using Robust.Shared.GameStates;

namespace Content.Shared._CM14.Surgery.Conditions;

[RegisterComponent, NetworkedComponent]
public sealed partial class CMSurgeryPartConditionComponent : Component
{
    [DataField]
    public BodyPartType Part;
}
