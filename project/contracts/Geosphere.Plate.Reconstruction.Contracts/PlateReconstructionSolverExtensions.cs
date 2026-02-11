using FantaSim.Geosphere.Plate.Kinematics.Contracts.Derived;
using FantaSim.Geosphere.Plate.Reconstruction.Contracts.Output;
using FantaSim.Geosphere.Plate.Reconstruction.Contracts.Policies;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Reconstruction.Contracts;

public static class PlateReconstructionSolverExtensions
{
    [Obsolete]
    public static IReadOnlyList<ReconstructedBoundary> ReconstructBoundaries(
        this IPlateReconstructionSolver solver,
        IPlateTopologyStateView topology,
        IPlateKinematicsStateView kinematics,
        CanonicalTick targetTick,
        ReconstructionOptions? options = null)
    {
        if (solver is null) throw new ArgumentNullException(nameof(solver));
        if (topology is null) throw new ArgumentNullException(nameof(topology));
        if (kinematics is null) throw new ArgumentNullException(nameof(kinematics));

        return solver.ReconstructBoundaries(
            topology,
            kinematics,
            ReconstructionPolicyDefaults.Visualization(ModelId.Default),
            targetTick,
            options);
    }
}
