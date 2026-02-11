namespace FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

/// <summary>
/// Well-known meta/governance domains.
/// </summary>
public static class MetaDomains
{
    /// <summary>
    /// Branch creation and fork lineage governance events.
    /// </summary>
    public static readonly MetaDomain BranchLineage = MetaDomain.Parse("meta.branch.lineage");
}
