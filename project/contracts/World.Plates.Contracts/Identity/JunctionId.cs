namespace Fantasim.World.Plates.Contracts.Identity;

/// <summary>
/// Strongly-typed identifier for a boundary junction (meeting point).
/// </summary>
public readonly record struct JunctionId(Guid Value)
{
    public static JunctionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
