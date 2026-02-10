using System.Collections.Immutable;
using FantaSim.World.Plates.Contracts;
using FantaSim.World.Plates.Contracts.Entities;
using FantaSim.World.Plates.Contracts.Events;
using FantaSim.World.Plates.Contracts.Identity;

namespace FantaSim.World.Plates.Topology;

/// <summary>
/// Internal mutable state that applies events. Exposes IPlateTopologyStateView.
/// </summary>
public sealed class PlateTopologyState : IPlateTopologyStateView
{
    private readonly Dictionary<PlateId, Plate> _plates = new();
    private readonly Dictionary<BoundaryId, Boundary> _boundaries = new();
    private readonly Dictionary<JunctionId, Junction> _junctions = new();

    public IReadOnlyDictionary<PlateId, Plate> Plates => _plates;
    public IReadOnlyDictionary<BoundaryId, Boundary> Boundaries => _boundaries;
    public IReadOnlyDictionary<JunctionId, Junction> Junctions => _junctions;
    public long LastEventSequence { get; private set; } = -1;

    public void Apply(IPlateTopologyEvent evt)
    {
        switch (evt)
        {
            case PlateCreatedEvent e:
                _plates[e.PlateId] = new Plate(e.PlateId);
                break;

            case PlateRetiredEvent e:
                if (_plates.TryGetValue(e.PlateId, out var plate))
                    _plates[e.PlateId] = plate with { IsRetired = true };
                break;

            case BoundaryCreatedEvent e:
                _boundaries[e.BoundaryId] = new Boundary(
                    e.BoundaryId, e.PlateIdLeft, e.PlateIdRight, e.BoundaryType);
                break;

            case BoundaryRetiredEvent e:
                if (_boundaries.TryGetValue(e.BoundaryId, out var boundary))
                    _boundaries[e.BoundaryId] = boundary with { IsRetired = true };
                break;

            case BoundaryTypeChangedEvent e:
                if (_boundaries.TryGetValue(e.BoundaryId, out var b))
                    _boundaries[e.BoundaryId] = b with { BoundaryType = e.NewBoundaryType };
                break;

            case JunctionCreatedEvent e:
                _junctions[e.JunctionId] = new Junction(e.JunctionId, e.BoundaryIds);
                break;
        }

        LastEventSequence = evt.Sequence;
    }
}
