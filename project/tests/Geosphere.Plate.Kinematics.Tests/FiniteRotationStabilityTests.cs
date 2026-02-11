using FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;
using Xunit;

namespace FantaSim.Geosphere.Plate.Kinematics.Tests;

/// <summary>
/// Tests for RFC-V2-0046 Section 6.3: Floating-point stability for rotation composition.
/// </summary>
public sealed class FiniteRotationStabilityTests
{
    private const double PiOver4 = Math.PI / 4.0;
    private const double PiOver2 = Math.PI / 2.0;

    [Fact]
    public void StableCompose_TwoValidRotations_ReturnsNormalized()
    {
        var rot1 = FiniteRotation.FromAxisAngle(Vector3d.UnitZ, PiOver4);
        var rot2 = FiniteRotation.FromAxisAngle(Vector3d.UnitZ, PiOver4);

        var result = rot1.StableCompose(rot2);

        var length = QuaternionLength(result.Orientation);
        Assert.True(Math.Abs(length - 1.0) < FiniteRotation.QuaternionTolerance,
            $"Quaternion length was {length}, expected ~1.0");
    }

    [Fact]
    public void StableCompose_ProducesUnitQuaternion()
    {
        var rot1 = FiniteRotation.FromAxisAngle(Vector3d.UnitX, PiOver4);
        var rot2 = FiniteRotation.FromAxisAngle(Vector3d.UnitY, PiOver2);

        var result = rot1.StableCompose(rot2);

        var length = QuaternionLength(result.Orientation);
        Assert.Equal(1.0, length, precision: 10);
    }

    [Fact]
    public void StableCompose_NumericalDrift_Corrects()
    {
        var smallRotation = FiniteRotation.FromAxisAngle(Vector3d.UnitZ, 0.01);
        var accumulated = FiniteRotation.Identity;

        for (int i = 0; i < 1000; i++)
        {
            accumulated = accumulated.StableCompose(smallRotation);
        }

        var length = QuaternionLength(accumulated.Orientation);
        Assert.True(Math.Abs(length - 1.0) < FiniteRotation.QuaternionTolerance,
            $"After 1000 compositions, quaternion length was {length}, expected ~1.0");
    }

    [Fact]
    public void StableCompose_Identity_ReturnsOriginal()
    {
        var rotation = FiniteRotation.FromAxisAngle(Vector3d.UnitX, PiOver2);

        var result = rotation.StableCompose(FiniteRotation.Identity);

        Assert.Equal(rotation.Angle, result.Angle, precision: 10);
    }

    [Fact]
    public void StableCompose_InverseRotation_ReturnsIdentity()
    {
        var rotation = FiniteRotation.FromAxisAngle(Vector3d.UnitY, PiOver4);
        var inverse = rotation.Inverted();

        var result = rotation.StableCompose(inverse);

        Assert.True(result.IsIdentity, "Composing with inverse should yield identity");
    }

    [Fact]
    public void StableInverted_ReturnsValidInverse()
    {
        var rotation = FiniteRotation.FromAxisAngle(Vector3d.UnitZ, PiOver2);

        var inverted = rotation.StableInverted();

        var composed = rotation.Compose(inverted);
        Assert.True(composed.IsIdentity, "Rotation composed with its stable inverse should be identity");
    }

    [Fact]
    public void StableInverted_ProducesUnitQuaternion()
    {
        var rotation = FiniteRotation.FromAxisAngle(
            new Vector3d(1, 1, 1).Normalize(),
            PiOver4);

        var inverted = rotation.StableInverted();

        var length = QuaternionLength(inverted.Orientation);
        Assert.Equal(1.0, length, precision: 10);
    }

    [Fact]
    public void StableInverted_Identity_ReturnsIdentity()
    {
        var identity = FiniteRotation.Identity;

        var inverted = identity.StableInverted();

        Assert.True(inverted.IsIdentity);
    }

    [Fact]
    public void StableInverted_DoubleInversion_ReturnsOriginal()
    {
        var rotation = FiniteRotation.FromAxisAngle(Vector3d.UnitX, PiOver4);

        var doubleInverted = rotation.StableInverted().StableInverted();

        Assert.Equal(rotation.Angle, doubleInverted.Angle, precision: 10);
    }

    [Fact]
    public void Tolerance_1e6_IsCorrectValue()
    {
        Assert.Equal(1e-6, FiniteRotation.QuaternionTolerance);
        Assert.True(FiniteRotation.QuaternionTolerance < 1e-5);
        Assert.True(FiniteRotation.QuaternionTolerance > 1e-15);
    }

    private static double QuaternionLength(Quaterniond q)
    {
        return Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);
    }
}
