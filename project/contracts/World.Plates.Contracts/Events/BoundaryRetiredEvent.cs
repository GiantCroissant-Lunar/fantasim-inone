using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Events;

public readonly record struct BoundaryRetiredEvent(
    Guid EventId,
    BoundaryId BoundaryId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(BoundaryRetiredEvent);
}
