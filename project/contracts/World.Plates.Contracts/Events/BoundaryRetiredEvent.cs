using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Events;

public readonly record struct BoundaryRetiredEvent(
    Guid EventId,
    BoundaryId BoundaryId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(BoundaryRetiredEvent);
}
