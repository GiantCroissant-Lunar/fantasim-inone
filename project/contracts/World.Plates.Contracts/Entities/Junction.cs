using System.Collections.Immutable;
using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Entities;

/// <summary>
/// A meeting point where three or more boundaries converge.
/// </summary>
public readonly record struct Junction(
    JunctionId JunctionId,
    ImmutableArray<BoundaryId> BoundaryIds,
    bool IsRetired = false
);
