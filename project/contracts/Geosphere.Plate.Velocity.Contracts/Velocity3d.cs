using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;
using FantaSim.Geosphere.Plate.Topology.Contracts.Numerics;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Velocity3d(
    [property: UnifyProperty(0)] double X,
    [property: UnifyProperty(1)] double Y,
    [property: UnifyProperty(2)] double Z)
{
    public static Velocity3d Zero => new(0, 0, 0);

    public double Magnitude() => Math.Sqrt(X * X + Y * Y + Z * Z);

    public double MagnitudeSquared() => X * X + Y * Y + Z * Z;

    public double Dot(Vector3d direction) => X * direction.X + Y * direction.Y + Z * direction.Z;

    public static Velocity3d operator +(Velocity3d a, Velocity3d b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Velocity3d operator -(Velocity3d a, Velocity3d b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Velocity3d operator -(Velocity3d v)
        => new(-v.X, -v.Y, -v.Z);

    public static Velocity3d operator *(Velocity3d v, double scalar)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static Velocity3d operator *(double scalar, Velocity3d v)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static Velocity3d operator /(Velocity3d v, double scalar)
        => new(v.X / scalar, v.Y / scalar, v.Z / scalar);

    public override string ToString() => $"Velocity3d({X:G6}, {Y:G6}, {Z:G6})";
}
