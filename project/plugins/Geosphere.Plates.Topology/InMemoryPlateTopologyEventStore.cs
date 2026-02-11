using FantaSim.Geosphere.Plates.Contracts;
using FantaSim.Geosphere.Plates.Contracts.Events;

namespace FantaSim.Geosphere.Plates.Topology;

/// <summary>
/// In-memory event store with sequence monotonicity validation.
/// </summary>
public sealed class InMemoryPlateTopologyEventStore : IPlateTopologyEventStore
{
    private readonly List<IPlateTopologyEvent> _events = [];
    private long _lastSequence = -1;

    public void Append(IPlateTopologyEvent evt)
    {
        if (evt.Sequence <= _lastSequence)
        {
            throw new InvalidOperationException(
                $"Sequence {evt.Sequence} is not greater than last sequence {_lastSequence}. Events must have strictly increasing sequences.");
        }

        _lastSequence = evt.Sequence;
        _events.Add(evt);
    }

    public IReadOnlyList<IPlateTopologyEvent> ReadAll()
    {
        return _events.ToList();
    }

    public IReadOnlyList<IPlateTopologyEvent> ReadUpToSequence(long targetSequence)
    {
        return _events.Where(e => e.Sequence <= targetSequence).ToList();
    }

    public IReadOnlyList<IPlateTopologyEvent> ReadUpToTick(long targetTick)
    {
        return _events.Where(e => e.Tick <= targetTick).ToList();
    }
}
