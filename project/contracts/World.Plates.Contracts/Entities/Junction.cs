using System.Collections.Immutable;
using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Entities;

/// <summary>
/// A meeting point where three or more boundaries converge.
/// </summary>
public readonly record struct Junction(
    JunctionId JunctionId,
    ImmutableArray<BoundaryId> BoundaryIds,
    bool IsRetired = false
);
