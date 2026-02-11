using System;
using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

/// <summary>
/// Stable identifier for meta/governance domains.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct MetaDomain(
    [property: UnifyProperty(0)] string Value)
{
    public static MetaDomain Parse(string value)
    {
        if (!TryParse(value, out var domain))
            throw new ArgumentException($"Invalid MetaDomain: '{value}'", nameof(value));

        return domain;
    }

    public static bool TryParse(string value, out MetaDomain domain)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            domain = default;
            return false;
        }

        foreach (var c in value)
        {
            if (!char.IsLetterOrDigit(c) && c != '.' && c != '_')
            {
                domain = default;
                return false;
            }
        }

        if (value.Contains("..", StringComparison.Ordinal) || value.StartsWith('.') || value.EndsWith('.'))
        {
            domain = default;
            return false;
        }

        domain = new MetaDomain(value);
        return true;
    }

    public bool IsValid() => TryParse(Value, out _);

    public override string ToString() => Value ?? string.Empty;

    public static explicit operator MetaDomain(string value) => Parse(value);
    public static implicit operator string(MetaDomain domain) => domain.Value;
}
