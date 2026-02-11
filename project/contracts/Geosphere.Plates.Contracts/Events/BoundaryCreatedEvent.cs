using FantaSim.Geosphere.Plates.Contracts.Entities;
using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts.Events;

public readonly record struct BoundaryCreatedEvent(
    Guid EventId,
    BoundaryId BoundaryId,
    PlateId PlateIdLeft,
    PlateId PlateIdRight,
    BoundaryType BoundaryType,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(BoundaryCreatedEvent);
}
