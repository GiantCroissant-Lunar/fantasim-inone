using System.Runtime.InteropServices;
using UnifySerialization.Abstractions;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

/// <summary>
/// Identifies a governance/meta stream scoped to a variant.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[UnifyModel]
public readonly record struct MetaStreamIdentity(
    [property: UnifyProperty(0)] string VariantId,
    [property: UnifyProperty(1)] MetaDomain Domain)
{
    public bool IsValid()
        => !string.IsNullOrWhiteSpace(VariantId) && Domain.IsValid();

    public override string ToString() => $"urn:fantasim:meta:{VariantId}:{Domain}";
}
