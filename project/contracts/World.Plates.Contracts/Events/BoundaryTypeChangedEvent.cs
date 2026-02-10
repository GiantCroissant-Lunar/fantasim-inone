using Fantasim.World.Plates.Contracts.Entities;
using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Events;

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
