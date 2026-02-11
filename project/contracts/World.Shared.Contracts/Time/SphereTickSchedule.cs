using Plate.TimeDete.Time.Primitives;

namespace FantaSim.World.Contracts.Time;

public sealed record SphereTickRatePeriod(
    CanonicalTick StartInclusive,
    CanonicalTick? EndExclusive,
    IReadOnlyDictionary<SphereId, TickScale> TicksPerCanonicalTick);

public sealed class SphereTickSchedule
{
    public SphereTickSchedule(IReadOnlyList<SphereTickRatePeriod> periods)
    {
        Periods = periods ?? throw new ArgumentNullException(nameof(periods));
        if (periods.Count == 0)
        {
            throw new ArgumentException("Schedule must contain at least one period.", nameof(periods));
        }
    }

    public IReadOnlyList<SphereTickRatePeriod> Periods { get; }

    public TickScale GetTicksPerCanonicalTick(SphereId sphere, CanonicalTick tick)
    {
        if (string.IsNullOrWhiteSpace(sphere.Value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(sphere));
        }

        for (var i = 0; i < Periods.Count; i++)
        {
            var p = Periods[i];
            if (tick.Value < p.StartInclusive.Value)
            {
                continue;
            }

            if (p.EndExclusive != null && tick.Value >= p.EndExclusive.Value.Value)
            {
                continue;
            }

            if (p.TicksPerCanonicalTick.TryGetValue(sphere, out var scale))
            {
                return scale;
            }

            throw new KeyNotFoundException(
                $"Sphere '{sphere.Value}' not found in tick schedule period starting at {p.StartInclusive.Value}.");
        }

        throw new InvalidOperationException($"No tick schedule period matched canonical tick {tick.Value}.");
    }
}
