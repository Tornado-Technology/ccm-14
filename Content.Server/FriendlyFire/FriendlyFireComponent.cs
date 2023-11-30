namespace Content.Server.FriendlyFire;

[RegisterComponent, Access(typeof(FriendlyFireSystem))]
public sealed partial class FriendlyFireComponent : Component
{
    [DataField]
    public bool Enabled;
}
