using FantaSim.Geosphere.Plate.Polygonization.Contracts.Products;

namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.Solvers;

public class PolygonizationException : Exception
{
    public PolygonizationDiagnostics Diagnostics { get; }

    public PolygonizationException(string message, PolygonizationDiagnostics diagnostics)
        : base(message)
    {
        Diagnostics = diagnostics;
    }

    public PolygonizationException(string message, PolygonizationDiagnostics diagnostics, Exception innerException)
        : base(message, innerException)
    {
        Diagnostics = diagnostics;
    }
}
