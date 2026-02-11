using Plate.TimeDete.Time.Primitives;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// Provisional sandbox export artifact used to propose canonical mutations.
/// This is derived data and is never canonical truth by itself.
/// </summary>
public sealed record CanonProposalBundle(
    string ProposalId,
    string TargetVariant,
    BranchId TargetBranch,
    CanonicalTick BaseTick,
    IReadOnlyList<DomainBaseHead> BaseHeads,
    IReadOnlyList<Domain> AffectedDomains,
    IReadOnlyList<ProposedChange> Changes,
    ProposalProvenance Provenance,
    bool RequestsFork = false
)
{
    /// <summary>
    /// Validates local bundle invariants.
    /// </summary>
    public IReadOnlyList<CanonProposalValidationIssue> ValidateInvariants()
    {
        var issues = new List<CanonProposalValidationIssue>();

        if (string.IsNullOrWhiteSpace(ProposalId))
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.InvalidProposalShape,
                "ProposalId is required."));
        }

        if (string.IsNullOrWhiteSpace(TargetVariant))
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.UnknownVariant,
                "TargetVariant is required."));
        }

        if (!TargetBranch.IsValid())
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.BranchMissingWithoutForkRequest,
                "TargetBranch is required."));
        }

        if (AffectedDomains is null || Changes is null || BaseHeads is null)
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.InvalidProposalShape,
                "BaseHeads, AffectedDomains, and Changes must be provided."));

            return issues;
        }

        var affectedDomainSet = new HashSet<Domain>(AffectedDomains);

        foreach (var change in Changes)
        {
            if (!affectedDomainSet.Contains(change.Domain))
            {
                issues.Add(new CanonProposalValidationIssue(
                    CanonProposalRejectionReason.UndeclaredDomainMutation,
                    $"Changed domain '{change.Domain}' is not declared in AffectedDomains.",
                    change.Domain));
            }
        }

        return issues;
    }
}
