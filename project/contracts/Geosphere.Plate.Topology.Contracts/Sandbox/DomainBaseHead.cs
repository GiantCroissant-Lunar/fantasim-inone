using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// Per-domain stream head observed by the sandbox when a proposal session started.
/// </summary>
public readonly record struct DomainBaseHead(
    Domain Domain,
    StreamHead Head
);
