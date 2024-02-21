using Robust.Shared.Utility;

namespace Content.Server._CM14.FriendlyFire;

[RegisterComponent]
public sealed partial class FriendlyFireToggleableActionComponent : Component
{
    [DataField]
    public SpriteSpecifier.Rsi IconOn = new(new ResPath("/Textures/_CM14/Interface/Actions/precision_shooting.rsi"), "on");

    [DataField]
    public SpriteSpecifier.Rsi IconOff = new(new ResPath("/Textures/_CM14/Interface/Actions/precision_shooting.rsi"), "off");
}
