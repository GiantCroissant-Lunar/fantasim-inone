using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Events;

public readonly record struct PlateCreatedEvent(
    Guid EventId,
    PlateId PlateId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(PlateCreatedEvent);
}
