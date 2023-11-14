namespace Content.Server.Fluids.EntitySystems;

[RegisterComponent]
public sealed partial class SplashOnTriggerComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Quantity = 100f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int SpreadAmount = 9;
}
