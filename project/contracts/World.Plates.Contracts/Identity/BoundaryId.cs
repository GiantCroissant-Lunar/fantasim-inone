namespace FantaSim.World.Plates.Contracts.Identity;

/// <summary>
/// Strongly-typed identifier for a plate boundary.
/// </summary>
public readonly record struct BoundaryId(Guid Value)
{
    public static BoundaryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
