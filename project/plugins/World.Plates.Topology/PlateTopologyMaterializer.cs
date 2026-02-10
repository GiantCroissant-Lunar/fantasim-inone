using Fantasim.World.Plates.Contracts;

namespace Fantasim.World.Plates.Topology;

/// <summary>
/// Replays events from the store to produce a materialized state view.
/// Deterministic: same events → identical state.
/// </summary>
public sealed class PlateTopologyMaterializer : IPlateTopologyMaterializer
{
    private readonly IPlateTopologyEventStore _store;

    public PlateTopologyMaterializer(IPlateTopologyEventStore store)
    {
        _store = store;
    }

    public IPlateTopologyStateView Materialize()
    {
        var state = new PlateTopologyState();
        foreach (var evt in _store.ReadAll())
        {
            state.Apply(evt);
        }
        return state;
    }

    public IPlateTopologyStateView MaterializeAtSequence(long targetSequence)
    {
        var state = new PlateTopologyState();
        foreach (var evt in _store.ReadUpToSequence(targetSequence))
        {
            state.Apply(evt);
        }
        return state;
    }

    public IPlateTopologyStateView MaterializeAtTick(long targetTick)
    {
        var state = new PlateTopologyState();
        foreach (var evt in _store.ReadUpToTick(targetTick))
        {
            state.Apply(evt);
        }
        return state;
    }
}
