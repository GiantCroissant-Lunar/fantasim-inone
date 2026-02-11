using System;
using MessagePack;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Kinematics.Contracts.Numerics;

/// <summary>
/// Thrown when numerical operations produce results outside acceptable tolerance bounds.
/// </summary>
public sealed class NumericalInstabilityException : InvalidOperationException
{
    public NumericalInstabilityException(string message) : base(message)
    {
    }
}

/// <summary>
/// Represents a finite rotation in 3D space.
/// </summary>
[MessagePackObject]
public readonly record struct FiniteRotation
{
    /// <summary>
    /// Tolerance for validating quaternion unit length after composition.
    /// </summary>
    public const double QuaternionTolerance = 1e-6;

    [Key(0)]
    public Quaterniond Orientation { get; init; }

    [SerializationConstructor]
    public FiniteRotation(Quaterniond orientation)
    {
        Orientation = Normalize(orientation);
    }

    public static FiniteRotation Identity => new FiniteRotation(Quaterniond.Identity);

    [IgnoreMember]
    public bool IsIdentity
    {
        get
        {
            const double eps = 1e-12;
            var q = Orientation;
            var sameHemisphere = q.W >= 0 ? q : new Quaterniond(-q.X, -q.Y, -q.Z, -q.W);
            return Math.Abs(sameHemisphere.X) < eps &&
                   Math.Abs(sameHemisphere.Y) < eps &&
                   Math.Abs(sameHemisphere.Z) < eps &&
                   Math.Abs(sameHemisphere.W - 1.0) < eps;
        }
    }

    public FiniteRotation Inverted()
    {
        return new FiniteRotation(Orientation.Inverse());
    }

    public FiniteRotation Compose(FiniteRotation other)
    {
        return new FiniteRotation(Quaterniond.Multiply(other.Orientation, Orientation));
    }

    public FiniteRotation StableCompose(FiniteRotation other)
    {
        var composed = Quaterniond.Multiply(other.Orientation, Orientation);
        var normalized = Normalize(composed);

        var length = Math.Sqrt(
            (normalized.X * normalized.X) +
            (normalized.Y * normalized.Y) +
            (normalized.Z * normalized.Z) +
            (normalized.W * normalized.W));

        if (Math.Abs(length - 1.0) > QuaternionTolerance)
        {
            throw new NumericalInstabilityException(
                "Rotation composition resulted in invalid quaternion");
        }

        return new FiniteRotation(normalized);
    }

    public FiniteRotation StableInverted()
    {
        var inverted = Orientation.Inverse();
        var normalized = Normalize(inverted);

        var length = Math.Sqrt(
            (normalized.X * normalized.X) +
            (normalized.Y * normalized.Y) +
            (normalized.Z * normalized.Z) +
            (normalized.W * normalized.W));

        if (Math.Abs(length - 1.0) > QuaternionTolerance)
        {
            throw new NumericalInstabilityException(
                "Rotation inversion resulted in invalid quaternion");
        }

        return new FiniteRotation(normalized);
    }

    [IgnoreMember]
    public Vector3d Axis
    {
        get
        {
            var q = Orientation;
            var w = Math.Clamp(q.W, -1.0, 1.0);
            var angle = 2.0 * Math.Acos(w);
            var sin = Math.Sin(angle / 2.0);
            if (Math.Abs(sin) < 1e-9) return Vector3d.UnitZ;
            return new Vector3d(q.X / sin, q.Y / sin, q.Z / sin).Normalize();
        }
    }

    [IgnoreMember]
    public double Angle => 2.0 * Math.Acos(Math.Clamp(Orientation.W, -1.0, 1.0));

    public static FiniteRotation FromAxisAngle(Vector3d axis, double angle)
    {
        var normalizedAxis = axis.Normalize();
        return new FiniteRotation(Quaterniond.FromAxisAngle(normalizedAxis, angle));
    }

    private static Quaterniond Normalize(Quaterniond q)
    {
        var norm = Math.Sqrt((q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W));
        if (norm <= double.Epsilon)
            return Quaterniond.Identity;
        return new Quaterniond(q.X / norm, q.Y / norm, q.Z / norm, q.W / norm);
    }
}
