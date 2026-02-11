using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Capabilities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Derived;

public interface IPlateTopologyMaterializationService
{
    Task<IPlateTopologyStateView> MaterializeAtTickAsync(
        TruthStreamIdentity stream,
        CanonicalTick targetTick,
        TickMaterializationMode mode = TickMaterializationMode.Auto,
        CancellationToken cancellationToken = default);
}
