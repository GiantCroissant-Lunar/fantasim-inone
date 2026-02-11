namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// Provisional provenance metadata for sandbox proposals.
///
/// Note: exact schema is deferred by RFC-V2-0072.
/// </summary>
public sealed record ProposalProvenance(
    string SandboxSystem,
    string SandboxSessionId,
    DateTimeOffset ExportedAtUtc,
    string ExportedBy
);
