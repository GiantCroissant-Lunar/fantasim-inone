using FantaSim.World.Plates.Contracts.Entities;
using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Events;

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
