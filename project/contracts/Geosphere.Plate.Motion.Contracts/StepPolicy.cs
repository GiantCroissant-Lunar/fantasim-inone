using MessagePack;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Motion.Contracts;

[Union(0, typeof(Adaptive))]
[Union(1, typeof(FixedInterval))]
[Union(2, typeof(BoundaryCrossing))]
[Union(3, typeof(KinematicDiscontinuity))]
public abstract record StepPolicy
{
    protected StepPolicy() { }

    public static StepPolicy Default { get; } = new FixedInterval(1.0);
}

[UnifyModel]
public sealed record Adaptive(
    [property: UnifyProperty(0)] double MinStepTicks,
    [property: UnifyProperty(1)] double MaxStepTicks,
    [property: UnifyProperty(2)] double Tolerance
) : StepPolicy;

[UnifyModel]
public sealed record FixedInterval(
    [property: UnifyProperty(0)] double StepTicks
) : StepPolicy;

[UnifyModel]
public sealed record BoundaryCrossing : StepPolicy;

[UnifyModel]
public sealed record KinematicDiscontinuity : StepPolicy;
