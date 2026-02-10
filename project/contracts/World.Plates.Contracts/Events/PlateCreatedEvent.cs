using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Events;

public readonly record struct PlateCreatedEvent(
    Guid EventId,
    PlateId PlateId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(PlateCreatedEvent);
}
