using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;

public interface IPlateKinematicsMaterializationService
{
    Task<IPlateKinematicsStateView> MaterializeAsync(
        TruthStreamIdentity stream,
        CancellationToken cancellationToken = default);
}
