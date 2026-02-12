using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Contracts.Observations;

/// <summary>
/// Emits observation artifacts from the simulation loop.
/// </summary>
public interface IObservationArtifactEmitter
{
    /// <summary>
    /// Emits observations for a completed tick.
    /// </summary>
    Task EmitAsync(
        TruthStreamIdentity stream,
        CanonicalTick tick,
        IReadOnlyList<ITruthEventDraft> events,
        CancellationToken ct = default);
}
