using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Entities;

/// <summary>
/// A tectonic plate entity.
/// </summary>
public readonly record struct Plate(
    PlateId PlateId,
    bool IsRetired = false
);
