using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct AngularVelocity3d(
    [property: UnifyProperty(0)] double X,
    [property: UnifyProperty(1)] double Y,
    [property: UnifyProperty(2)] double Z)
{
    public static AngularVelocity3d Zero => new(0, 0, 0);

    public double Rate() => Math.Sqrt(X * X + Y * Y + Z * Z);

    public double RateSquared() => X * X + Y * Y + Z * Z;

    public (double AxisX, double AxisY, double AxisZ) GetAxis()
    {
        var rate = Rate();
        if (rate < double.Epsilon)
            return (0, 0, 0);
        return (X / rate, Y / rate, Z / rate);
    }

    public Velocity3d GetLinearVelocityAt(double pointX, double pointY, double pointZ)
    {
        return new Velocity3d(
            Y * pointZ - Z * pointY,
            Z * pointX - X * pointZ,
            X * pointY - Y * pointX);
    }

    public static AngularVelocity3d operator +(AngularVelocity3d a, AngularVelocity3d b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static AngularVelocity3d operator -(AngularVelocity3d a, AngularVelocity3d b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static AngularVelocity3d operator -(AngularVelocity3d v)
        => new(-v.X, -v.Y, -v.Z);

    public static AngularVelocity3d operator *(AngularVelocity3d v, double scalar)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static AngularVelocity3d operator *(double scalar, AngularVelocity3d v)
        => new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static AngularVelocity3d operator /(AngularVelocity3d v, double scalar)
        => new(v.X / scalar, v.Y / scalar, v.Z / scalar);

    public override string ToString() => $"AngularVelocity3d({X:G6}, {Y:G6}, {Z:G6})";
}
