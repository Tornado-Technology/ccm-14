namespace Content.Server._CM14.MapDay;

[RegisterComponent]
public sealed partial class MapDayComponent : Component
{
    public TimeSpan DayTime => TimeSpan.FromSeconds(DayTimeSeconds);

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public double DayTimeSeconds = TimeSpan.FromMinutes(40).TotalSeconds;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public List<Color> Colors = new()
    {
        new Color(0, 0, 0),
        new Color(0, 0, 0),
        new Color(10, 10, 10),
        new Color(10, 10, 10),
        new Color(55, 66, 74),
        new Color(140, 152, 160),
        new Color(140, 152, 160),
        new Color(140, 152, 160),
        new Color(55, 66, 74),
        new Color(0, 0, 0),
    };
}
