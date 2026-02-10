using FantaSim.World.Plates.Contracts.Events;

namespace FantaSim.World.Plates.Contracts;

/// <summary>
/// Append-only event store for plate topology events.
/// </summary>
public interface IPlateTopologyEventStore
{
    void Append(IPlateTopologyEvent evt);
    IReadOnlyList<IPlateTopologyEvent> ReadAll();
    IReadOnlyList<IPlateTopologyEvent> ReadUpToSequence(long targetSequence);
    IReadOnlyList<IPlateTopologyEvent> ReadUpToTick(long targetTick);
}
