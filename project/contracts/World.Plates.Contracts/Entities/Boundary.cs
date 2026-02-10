using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Entities;

/// <summary>
/// A boundary between two tectonic plates.
/// </summary>
public readonly record struct Boundary(
    BoundaryId BoundaryId,
    PlateId PlateIdLeft,
    PlateId PlateIdRight,
    BoundaryType BoundaryType,
    bool IsRetired = false
);
