using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// RFC-V2-0072 minimum gate rejection reasons.
/// </summary>
public enum CanonProposalRejectionReason
{
    InvalidProposalShape = 0,
    StaleBaseHead = 1,
    UndeclaredDomainMutation = 2,
    UnknownVariant = 3,
    BranchOutsideVariant = 4,
    BranchMissingWithoutForkRequest = 5
}

/// <summary>
/// Validation issue encountered during proposal evaluation.
/// </summary>
public sealed record CanonProposalValidationIssue(
    CanonProposalRejectionReason Reason,
    string Message,
    Domain? Domain = null
);

/// <summary>
/// Context required by the validator to evaluate canonical state.
/// </summary>
public sealed record CanonProposalValidationContext(
    bool TargetVariantExists,
    bool TargetBranchBelongsToVariant,
    bool TargetBranchExists,
    IReadOnlyDictionary<Domain, StreamHead> CurrentHeads
);

/// <summary>
/// Result of proposal validation.
/// </summary>
public sealed record CanonProposalValidationResult(
    bool IsAccepted,
    IReadOnlyList<CanonProposalValidationIssue> Issues
);

/// <summary>
/// Validates sandbox proposal bundles against RFC-V2-0072 minimum gates.
/// </summary>
public interface ICanonProposalValidator
{
    CanonProposalValidationResult Validate(
        CanonProposalBundle proposal,
        CanonProposalValidationContext context);
}

/// <summary>
/// Default minimum-gate validator implementation.
/// </summary>
public sealed class CanonProposalValidator : ICanonProposalValidator
{
    public CanonProposalValidationResult Validate(
        CanonProposalBundle proposal,
        CanonProposalValidationContext context)
    {
        var issues = new List<CanonProposalValidationIssue>();

        issues.AddRange(proposal.ValidateInvariants());

        if (!context.TargetVariantExists)
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.UnknownVariant,
                $"Target variant '{proposal.TargetVariant}' does not exist."));
        }

        if (!context.TargetBranchBelongsToVariant)
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.BranchOutsideVariant,
                $"Target branch '{proposal.TargetBranch}' does not belong to variant '{proposal.TargetVariant}'."));
        }

        if (!proposal.RequestsFork && !context.TargetBranchExists)
        {
            issues.Add(new CanonProposalValidationIssue(
                CanonProposalRejectionReason.BranchMissingWithoutForkRequest,
                $"Target branch '{proposal.TargetBranch}' does not exist and proposal does not request fork."));
        }

        foreach (var baseHead in proposal.BaseHeads)
        {
            if (!context.CurrentHeads.TryGetValue(baseHead.Domain, out var currentHead))
            {
                issues.Add(new CanonProposalValidationIssue(
                    CanonProposalRejectionReason.StaleBaseHead,
                    $"Current stream head for domain '{baseHead.Domain}' is unavailable.",
                    baseHead.Domain));
                continue;
            }

            if (!currentHead.Equals(baseHead.Head))
            {
                issues.Add(new CanonProposalValidationIssue(
                    CanonProposalRejectionReason.StaleBaseHead,
                    $"Domain '{baseHead.Domain}' advanced since sandbox base head.",
                    baseHead.Domain));
            }
        }

        return new CanonProposalValidationResult(
            IsAccepted: issues.Count == 0,
            Issues: issues);
    }
}
