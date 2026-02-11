using FantaSim.Geosphere.Plate.Runtime.Des.Core;
using FantaSim.Geosphere.Plate.Runtime.Des.Contracts;
using FantaSim.World.Contracts.Time;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Runtime;

/// <summary>
/// DES scheduler that assigns monotonically increasing TieBreak values
/// to ensure deterministic ordering of work items.
/// </summary>
public sealed class DesScheduler : IDesScheduler
{
    private readonly IDesQueue _queue;
    private ulong _tieBreakCounter;

    public DesScheduler(IDesQueue queue)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _tieBreakCounter = 0;
    }

    /// <inheritdoc />
    public void Schedule(CanonicalTick when, SphereId sphere, DesWorkKind kind, object? payload = null)
    {
        var tieBreak = _tieBreakCounter++;
        var item = new ScheduledWorkItem(when, sphere, kind, tieBreak, payload);
        _queue.Enqueue(item);
    }
}
