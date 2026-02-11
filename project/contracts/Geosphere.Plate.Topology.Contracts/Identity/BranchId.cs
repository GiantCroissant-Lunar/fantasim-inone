using System;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

/// <summary>
/// Strongly-typed branch identifier.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct BranchId(
    [property: UnifyProperty(0)] string Value)
{
    /// <summary>
    /// Parses a branch identifier.
    /// </summary>
    public static BranchId Parse(string value)
    {
        if (!TryParse(value, out var branchId))
            throw new ArgumentException($"Invalid BranchId: '{value}'", nameof(value));

        return branchId;
    }

    /// <summary>
    /// Attempts to parse a branch identifier.
    /// </summary>
    public static bool TryParse(string value, out BranchId branchId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            branchId = default;
            return false;
        }

        foreach (var c in value)
        {
            if (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.')
            {
                branchId = default;
                return false;
            }
        }

        branchId = new BranchId(value);
        return true;
    }

    /// <summary>
    /// True when this identifier is well-formed.
    /// </summary>
    public bool IsValid() => TryParse(Value, out _);

    public override string ToString() => Value ?? string.Empty;

    public static explicit operator BranchId(string value) => Parse(value);
    public static implicit operator string(BranchId branchId) => branchId.Value;
}
