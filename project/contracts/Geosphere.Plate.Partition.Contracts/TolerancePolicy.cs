using System.Runtime.InteropServices;
using MessagePack;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Partition.Contracts;

[Union(0, typeof(StrictPolicy))]
[Union(1, typeof(LenientPolicy))]
[Union(2, typeof(PolygonizerDefaultPolicy))]
[StructLayout(LayoutKind.Auto)]
public abstract record TolerancePolicy
{
    protected TolerancePolicy() { }
}

[UnifyModel]
public sealed record StrictPolicy : TolerancePolicy
{
    public StrictPolicy() { }
}

[UnifyModel]
public sealed record LenientPolicy(
    [property: UnifyProperty(0)] double Epsilon
) : TolerancePolicy
{
    public LenientPolicy() : this(1e-9) { }
}

[UnifyModel]
public sealed record PolygonizerDefaultPolicy : TolerancePolicy
{
    public PolygonizerDefaultPolicy() { }
}
