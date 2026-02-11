using FantaSim.Geosphere.Plates.Contracts.Entities;
using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts.Events;

public readonly record struct BoundaryTypeChangedEvent(
    Guid EventId,
    BoundaryId BoundaryId,
    BoundaryType NewBoundaryType,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(BoundaryTypeChangedEvent);
}
