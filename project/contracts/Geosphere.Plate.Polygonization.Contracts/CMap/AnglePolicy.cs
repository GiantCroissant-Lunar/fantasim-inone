using System.Runtime.InteropServices;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Policy for comparing angles in a deterministic, stable manner.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly record struct AnglePolicy
{
    public double Epsilon { get; init; }
    public bool UseQuantization { get; init; }
    public double QuantizationRadians { get; init; }

    public static AnglePolicy Default => new()
    {
        Epsilon = 1e-12,
        UseQuantization = false,
        QuantizationRadians = 0
    };

    public static AnglePolicy Strict => new()
    {
        Epsilon = 0,
        UseQuantization = false,
        QuantizationRadians = 0
    };

    public static AnglePolicy Quantized(double binSizeRadians = 1e-9) => new()
    {
        Epsilon = 0,
        UseQuantization = true,
        QuantizationRadians = binSizeRadians
    };

    public int CompareAngles(double angleA, double angleB)
    {
        double a = angleA;
        double b = angleB;

        if (UseQuantization && QuantizationRadians > 0)
        {
            a = Math.Floor(a / QuantizationRadians) * QuantizationRadians;
            b = Math.Floor(b / QuantizationRadians) * QuantizationRadians;
        }

        var diff = a - b;

        if (Epsilon > 0 && Math.Abs(diff) <= Epsilon)
        {
            return 0;
        }

        return diff.CompareTo(0.0);
    }
}
