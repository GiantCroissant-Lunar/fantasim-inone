using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts.Entities;

/// <summary>
/// A tectonic plate entity.
/// </summary>
public readonly record struct Plate(
    PlateId PlateId,
    bool IsRetired = false
);
