using FantaSim.Geosphere.Plate.Kinematics.Contracts.Entities;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Events;
using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Kinematics.Materializer;
using FantaSim.Geosphere.Plate.Topology.Contracts.Entities;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;
using Plate.TimeDete.Time.Primitives;
using Xunit;

namespace FantaSim.Geosphere.Plate.Kinematics.Tests;

public sealed class KinematicsMaterializerTests
{
    private static readonly TruthStreamIdentity TestStream = new(
        "science", "trunk", 2, Domain.Parse("geo.plates.kinematics"), "0");

    private static readonly PlateId TestPlateId =
        new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    private static readonly MotionSegmentId TestSegmentId =
        new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Fact]
    public async Task MaterializeThenQuery_IsDeterministic()
    {
        var store = new InMemoryKinematicsEventStore();
        var materializer = new PlateKinematicsMaterializer(store);

        var rot = QuantizedEulerPoleRotation.Create(0, 0, 90 * QuantizedEulerPoleRotation.MicroDegPerDeg);
        var upsert = new MotionSegmentUpsertedEvent(
            Guid.NewGuid(),
            TestPlateId,
            TestSegmentId,
            new CanonicalTick(0),
            new CanonicalTick(10),
            rot,
            new CanonicalTick(0),
            0,
            TestStream,
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty);

        await store.AppendAsync(TestStream, new IPlateKinematicsEvent[] { upsert }, CancellationToken.None);

        var s1 = await materializer.MaterializeAsync(TestStream);
        var s2 = await materializer.MaterializeAsync(TestStream);

        Assert.True(s1.TryGetRotation(TestPlateId, new CanonicalTick(5), out var r1));
        Assert.True(s2.TryGetRotation(TestPlateId, new CanonicalTick(5), out var r2));

        Assert.Equal(r1, r2);
    }

    [Fact]
    public async Task Materialize_EmptyStore_ReturnsIdentityRotation()
    {
        var store = new InMemoryKinematicsEventStore();
        var materializer = new PlateKinematicsMaterializer(store);

        var state = await materializer.MaterializeAsync(TestStream);

        Assert.True(state.TryGetRotation(TestPlateId, new CanonicalTick(5), out var rotation));
        Assert.Equal(Topology.Contracts.Numerics.Quaterniond.Identity, rotation);
    }

    [Fact]
    public async Task Materialize_RetiredSegment_IsNotUsed()
    {
        var store = new InMemoryKinematicsEventStore();
        var materializer = new PlateKinematicsMaterializer(store);

        var rot = QuantizedEulerPoleRotation.Create(0, 0, 90 * QuantizedEulerPoleRotation.MicroDegPerDeg);
        var upsert = new MotionSegmentUpsertedEvent(
            Guid.NewGuid(), TestPlateId, TestSegmentId,
            new CanonicalTick(0), new CanonicalTick(10), rot,
            new CanonicalTick(0), 0, TestStream,
            ReadOnlyMemory<byte>.Empty, ReadOnlyMemory<byte>.Empty);

        var retire = new MotionSegmentRetiredEvent(
            Guid.NewGuid(), TestPlateId, TestSegmentId, "test-retire",
            new CanonicalTick(1), 1, TestStream,
            ReadOnlyMemory<byte>.Empty, ReadOnlyMemory<byte>.Empty);

        await store.AppendAsync(TestStream, new IPlateKinematicsEvent[] { upsert, retire }, CancellationToken.None);

        var state = await materializer.MaterializeAsync(TestStream);

        Assert.True(state.TryGetRotation(TestPlateId, new CanonicalTick(5), out var rotation));
        // With no active segments, rotation should be identity
        Assert.Equal(Topology.Contracts.Numerics.Quaterniond.Identity, rotation);
    }
}
