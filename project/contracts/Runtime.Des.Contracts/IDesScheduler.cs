using FantaSim.World.Contracts.Time;
using Plate.TimeDete.Time.Primitives;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Contracts;

public interface IDesScheduler
{
    void Schedule(CanonicalTick when, SphereId sphere, DesWorkKind kind, object? payload = null);
}
