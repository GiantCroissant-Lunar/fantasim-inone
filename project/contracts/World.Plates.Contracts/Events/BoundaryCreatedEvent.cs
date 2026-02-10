using Fantasim.World.Plates.Contracts.Entities;
using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Events;

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
