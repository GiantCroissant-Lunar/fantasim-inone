namespace FantaSim.Geosphere.Plates.Contracts.Identity;

/// <summary>
/// Identifies an authoritative event stream: (Variant, Branch, L, Domain, M).
/// Per RFC-V2-0001.
/// </summary>
public readonly record struct TruthStreamIdentity(
    string VariantId,
    string BranchId,
    int LLevel,
    string Domain,
    string Model
)
{
    public static TruthStreamIdentity Default => new("v1", "main", 2, "geo.plates.topology", "M0");

    public override string ToString() => $"{VariantId}:{BranchId}:L{LLevel}:{Domain}:{Model}";
}
