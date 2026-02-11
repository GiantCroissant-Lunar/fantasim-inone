using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// Provisional change descriptor exported by sandbox systems.
///
/// Note: detailed domain payload schema is deferred by RFC-V2-0072.
/// </summary>
public sealed record ProposedChange(
    Domain Domain,
    string ChangeKind,
    string Payload
);
