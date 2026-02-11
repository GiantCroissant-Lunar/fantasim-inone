namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

public class CMapBuildException : Exception
{
    public CMapBuildException(string message) : base(message) { }
    public CMapBuildException(string message, Exception innerException) : base(message, innerException) { }
}
