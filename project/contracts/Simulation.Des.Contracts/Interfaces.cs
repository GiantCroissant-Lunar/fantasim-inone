using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using Plate.TimeDete.Determinism.Abstractions;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Simulation.Des.Contracts;

/// <summary>
/// Marker interface for a DES driver.
/// </summary>
public interface IDriver;

/// <summary>
/// Marker interface for a DES trigger.
/// </summary>
public interface ITrigger;

/// <summary>
/// A driver that can be executed synchronously within the DES loop.
/// </summary>
public interface IExecutableDriver : IDriver
{
    DriverOutput Execute(DesContext context);
}

/// <summary>
/// A driver that evaluates asynchronously within the DES loop.
/// </summary>
public interface IAsyncDriver : IDriver
{
    Task<DriverOutput> EvaluateAsync(DesContext context, CancellationToken ct = default);
}

/// <summary>
/// A trigger that emits truth event drafts from a driver's output.
/// </summary>
public interface IExecutableTrigger : ITrigger
{
    IReadOnlyList<ITruthEventDraft> EmitDrafts(DriverOutput output, CanonicalTick tick, ISeededRng rng);
}

/// <summary>
/// A typed DES driver with specific output.
/// </summary>
public interface IDesDriver : IDriver
{
    DriverId DriverId { get; }
}

/// <summary>
/// A typed DES trigger with specific output.
/// </summary>
public interface IDesTrigger : ITrigger
{
    TriggerId TriggerId { get; }
}
