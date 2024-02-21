namespace Content.Server._CM14.FriendlyFire;

[RegisterComponent, Access(typeof(FriendlyFireSystem))]
public sealed partial class FriendlyFireComponent : Component
{
    [DataField]
    public bool Enabled;
}
