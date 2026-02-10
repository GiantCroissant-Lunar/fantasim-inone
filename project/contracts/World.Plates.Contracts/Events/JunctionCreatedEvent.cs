using System.Collections.Immutable;
using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Contracts.Events;

public readonly record struct JunctionCreatedEvent(
    Guid EventId,
    JunctionId JunctionId,
    ImmutableArray<BoundaryId> BoundaryIds,
    long Sequence,
    long Tick
) : IPlateTopologyEvent
{
    public string EventType => nameof(JunctionCreatedEvent);
}
