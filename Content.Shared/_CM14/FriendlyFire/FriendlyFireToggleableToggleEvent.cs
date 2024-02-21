namespace Content.Shared._CM14.FriendlyFire;

[ByRefEvent]
public struct FriendlyFireToggleableToggleEvent
{
    public bool Enabled;

    public FriendlyFireToggleableToggleEvent(bool enabled)
    {
        Enabled = enabled;
    }
}
