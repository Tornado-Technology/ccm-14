using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Server.TagInteraction;

[RegisterComponent]
public sealed partial class TagInteractionComponent : Component
{
    [DataField]
    public ProtoId<TagPrototype>? WhitelistTag;

    [DataField]
    public ProtoId<TagPrototype>? BlacklistTag;
}
