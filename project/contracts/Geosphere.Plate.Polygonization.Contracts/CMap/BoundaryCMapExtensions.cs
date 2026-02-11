namespace FantaSim.Geosphere.Plate.Polygonization.Contracts.CMap;

/// <summary>
/// Extension methods for face-walking on boundary cmaps.
/// RFC-V2-0041 §11.2.
/// </summary>
public static class BoundaryCMapExtensions
{
    public static IReadOnlyList<BoundaryDart> WalkFace(this IBoundaryCMap cmap, BoundaryDart start)
    {
        var result = new List<BoundaryDart>();
        var current = start;

        do
        {
            result.Add(current);
            current = cmap.Next(current);

            if (result.Count > 10_000)
            {
                throw new InvalidOperationException(
                    $"Face walk exceeded 10000 darts starting from {start}. Possible invalid cmap.");
            }
        }
        while (current != start);

        return result;
    }

    public static IEnumerable<IReadOnlyList<BoundaryDart>> EnumerateFaces(this IBoundaryCMap cmap)
    {
        var visited = new HashSet<BoundaryDart>();
        var sortedDarts = cmap.Darts.OrderBy(d => d).ToList();

        foreach (var dart in sortedDarts)
        {
            if (visited.Contains(dart))
                continue;

            var face = cmap.WalkFace(dart);

            foreach (var d in face)
                visited.Add(d);

            yield return face;
        }
    }
}
