using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.Events;

public enum KinematicsTickMonotonicityPolicy
{
    Allow = 0,
    Warn = 1,
    Reject = 2
}

public sealed class KinematicsAppendOptions
{
    public static readonly KinematicsAppendOptions Default = new();

    public KinematicsTickMonotonicityPolicy TickPolicy { get; init; } = KinematicsTickMonotonicityPolicy.Allow;
}

public interface IKinematicsEventStore
{
    Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateKinematicsEvent> events,
        CancellationToken cancellationToken);

    Task AppendAsync(
        TruthStreamIdentity stream,
        IEnumerable<IPlateKinematicsEvent> events,
        KinematicsAppendOptions options,
        CancellationToken cancellationToken);

    IAsyncEnumerable<IPlateKinematicsEvent> ReadAsync(
        TruthStreamIdentity stream,
        long fromSequenceInclusive,
        CancellationToken cancellationToken);

    Task<long?> GetLastSequenceAsync(
        TruthStreamIdentity stream,
        CancellationToken cancellationToken);
}
