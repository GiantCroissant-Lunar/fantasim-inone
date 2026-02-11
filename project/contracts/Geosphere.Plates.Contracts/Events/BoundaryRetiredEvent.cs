using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts.Events;

public readonly record struct BoundaryRetiredEvent(
    Guid EventId,
    BoundaryId BoundaryId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(BoundaryRetiredEvent);
}
