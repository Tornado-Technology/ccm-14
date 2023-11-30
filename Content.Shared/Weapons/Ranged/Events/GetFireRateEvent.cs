namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public struct GetFireRateEvent
{
    public float FireRate;

    public GetFireRateEvent(float fireRate)
    {
        FireRate = fireRate;
    }
}
