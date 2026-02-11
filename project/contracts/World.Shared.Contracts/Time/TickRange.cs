using Plate.TimeDete.Time.Primitives;

namespace FantaSim.World.Contracts.Time;

public readonly record struct TickRange
{
    public CanonicalTick StartInclusive { get; }

    public CanonicalTick EndExclusive { get; }

    public TickRange(CanonicalTick startInclusive, CanonicalTick endExclusive)
    {
        if (endExclusive.Value < startInclusive.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(endExclusive), "EndExclusive must be >= StartInclusive.");
        }

        StartInclusive = startInclusive;
        EndExclusive = endExclusive;
    }

    public TickDelta Length => new(checked(EndExclusive.Value - StartInclusive.Value));

    public bool Contains(CanonicalTick tick)
    {
        return tick.Value >= StartInclusive.Value && tick.Value < EndExclusive.Value;
    }
}
