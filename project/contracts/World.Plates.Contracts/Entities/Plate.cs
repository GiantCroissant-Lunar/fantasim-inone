using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Entities;

/// <summary>
/// A tectonic plate entity.
/// </summary>
public readonly record struct Plate(
    PlateId PlateId,
    bool IsRetired = false
);
