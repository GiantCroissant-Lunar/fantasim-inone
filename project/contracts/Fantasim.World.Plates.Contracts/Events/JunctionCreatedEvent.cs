using System.Collections.Immutable;
using Fantasim.World.Plates.Contracts.Identity;

namespace Fantasim.World.Plates.Contracts.Events;

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
