using FantaSim.World.Plates.Contracts;

namespace FantaSim.World.Plates.Topology;

/// <summary>
/// Validates plate topology invariants per RFC-V2-0001 §5.
/// </summary>
public static class PlateTopologyInvariantValidator
{
    public static IReadOnlyList<string> Validate(IPlateTopologyStateView state)
    {
        var violations = new List<string>();

        // Each boundary separates exactly two distinct plates
        foreach (var (id, boundary) in state.Boundaries)
        {
            if (boundary.IsRetired) continue;

            if (boundary.PlateIdLeft == boundary.PlateIdRight)
            {
                violations.Add($"Boundary {id} has identical left and right plates.");
            }

            if (!state.Plates.ContainsKey(boundary.PlateIdLeft))
            {
                violations.Add($"Boundary {id} references non-existent left plate {boundary.PlateIdLeft}.");
            }

            if (!state.Plates.ContainsKey(boundary.PlateIdRight))
            {
                violations.Add($"Boundary {id} references non-existent right plate {boundary.PlateIdRight}.");
            }
        }

        // No orphan junctions — all referenced boundaries must exist
        foreach (var (id, junction) in state.Junctions)
        {
            if (junction.IsRetired) continue;

            foreach (var boundaryId in junction.BoundaryIds)
            {
                if (!state.Boundaries.ContainsKey(boundaryId))
                {
                    violations.Add($"Junction {id} references non-existent boundary {boundaryId}.");
                }
            }
        }

        return violations;
    }
}
