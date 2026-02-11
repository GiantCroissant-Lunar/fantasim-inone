using FantaSim.Geosphere.Plates.Contracts.Identity;

namespace FantaSim.Geosphere.Plates.Contracts.Events;

public readonly record struct PlateRetiredEvent(
    Guid EventId,
    PlateId PlateId,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(PlateRetiredEvent);
}
