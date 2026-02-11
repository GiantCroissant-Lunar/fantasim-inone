namespace FantaSim.Geosphere.Plates.Contracts.Identity;

/// <summary>
/// Strongly-typed identifier for a tectonic plate.
/// </summary>
public readonly record struct PlateId(Guid Value)
{
    public static PlateId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
