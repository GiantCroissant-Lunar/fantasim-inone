using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Velocity.Contracts;

[UnifyModel]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct RateUncertainty(
    [property: UnifyProperty(0)] double NormalRateSigma,
    [property: UnifyProperty(1)] double TangentialRateSigma,
    [property: UnifyProperty(2)] double SpeedSigma,
    [property: UnifyProperty(3)] double NormalRateConfidence95Lower,
    [property: UnifyProperty(4)] double NormalRateConfidence95Upper,
    [property: UnifyProperty(5)] double TangentialRateConfidence95Lower,
    [property: UnifyProperty(6)] double TangentialRateConfidence95Upper
)
{
    [UnifyIgnore]
    public (double Lower, double Upper) NormalRateConfidence95 => (NormalRateConfidence95Lower, NormalRateConfidence95Upper);

    [UnifyIgnore]
    public (double Lower, double Upper) TangentialRateConfidence95 => (TangentialRateConfidence95Lower, TangentialRateConfidence95Upper);

    public static RateUncertainty FromSigmas(double normalSigma, double tangentialSigma, double speedSigma)
    {
        const double z95 = 1.96;
        return new RateUncertainty(
            NormalRateSigma: normalSigma,
            TangentialRateSigma: tangentialSigma,
            SpeedSigma: speedSigma,
            NormalRateConfidence95Lower: -z95 * normalSigma,
            NormalRateConfidence95Upper: z95 * normalSigma,
            TangentialRateConfidence95Lower: -z95 * tangentialSigma,
            TangentialRateConfidence95Upper: z95 * tangentialSigma
        );
    }
}
