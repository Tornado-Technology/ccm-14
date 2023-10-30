namespace Content.Server.Xeno.Actions.Components;

[RegisterComponent]
public sealed partial class XenoRejuvenateProjComponent : Component
{
    [DataField("healAmount")]
    public float HealAmount = 300f;
}
