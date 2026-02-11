namespace FantaSim.Space.Stellar.Contracts.Solvers;

/// <summary>
/// Defines insolation computation methods for stellar-planet interactions.
/// Provides deterministic, pure functions for calculating solar irradiance parameters.
/// </summary>
public interface IInsolationSolver
{
    /// <summary>
    /// Calculates solar flux at a given distance from a star using the inverse-square law.
    /// </summary>
    double CalculateSolarFlux(double starLuminosityW, double distanceM);

    /// <summary>
    /// Calculates daily insolation at a given latitude for a specific solar declination.
    /// </summary>
    double CalculateDailyInsolation(double latitudeRad, double declinationRad, double solarConstant);

    /// <summary>
    /// Calculates the solar zenith angle at a given location and time.
    /// </summary>
    double CalculateSolarZenithAngle(double latitudeRad, double declinationRad, double hourAngleRad);

    /// <summary>
    /// Calculates the length of daylight at a given latitude for a specific solar declination.
    /// </summary>
    double CalculateDayLength(double latitudeRad, double declinationRad);

    /// <summary>
    /// Calculates the sunrise hour angle, which determines when the sun crosses the horizon.
    /// </summary>
    double CalculateSunriseHourAngle(double latitudeRad, double declinationRad);
}
