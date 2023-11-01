namespace Content.Shared.Xeno;

[RegisterComponent]
public sealed partial class XenoTierComponent : Component
{
    [DataField("tier")]
    public int Tier;
}
