using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Server._CM14.MapDay;

public sealed class MapDaySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    private readonly TimeSpan _updateDelay = TimeSpan.FromSeconds(0.1f);
    private TimeSpan _updateNext;

    private bool _enabled = true;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_enabled)
            return;

        if (_updateNext > _timing.CurTime)
            return;

        _updateNext = _timing.CurTime + _updateDelay;

        var query = EntityQueryEnumerator<MapDayComponent, MapLightComponent>();
        while (query.MoveNext(out var uid, out var day, out var light))
        {
            var time = _timing.CurTime.TotalSeconds % day.DayTimeSeconds;
            var amount = time / day.DayTimeSeconds;
            var color = InterpolateColor(day.Colors, (float)amount);
            light.AmbientLightColor = color;
            Dirty(uid, light);
        }
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }

    private Color InterpolateColor(IReadOnlyList<Color> colors, float fraction)
    {
        if (colors.Count < 2)
            throw new ArgumentException("At least two colors required for interpolation.");

        var scale = fraction * (colors.Count - 1);
        var startIndex = (int)Math.Floor(scale);
        var endIndex = (int)Math.Ceiling(scale);

        var startColor = colors[startIndex];
        var endColor = colors[Math.Min(endIndex, colors.Count - 1)];

        var localFraction = scale - startIndex;
        var r = startColor.R + (endColor.R - startColor.R) * localFraction;
        var g = startColor.G + (endColor.G - startColor.G) * localFraction;
        var b = startColor.B + (endColor.B - startColor.B) * localFraction;

        return new Color(r, g, b);
    }
}
