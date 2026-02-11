using FantaSim.Geosphere.Plate.Topology.Contracts.Identity;

namespace FantaSim.Geosphere.Plate.Topology.Contracts.Sandbox;

/// <summary>
/// Result of attempting to ingest a sandbox proposal into canon.
/// </summary>
public sealed record CanonProposalIngestionResult(
    bool Accepted,
    IReadOnlyList<CanonProposalValidationIssue> Issues,
    IReadOnlyList<Domain> AcceptedDomains
);

/// <summary>
/// Canon-side ingestion seam for sandbox proposal bundles.
///
/// Implementations should validate proposals and translate accepted changes into
/// normal domain truth events (never special proposal events).
/// </summary>
public interface ICanonProposalIngester
{
    Task<CanonProposalIngestionResult> IngestAsync(
        CanonProposalBundle proposal,
        CanonProposalValidationContext context,
        CancellationToken cancellationToken);
}

/// <summary>
/// Minimal ingestion implementation that performs RFC minimum-gate validation
/// and returns acceptance/rejection outcome.
/// </summary>
public sealed class CanonProposalIngester(ICanonProposalValidator validator)
    : ICanonProposalIngester
{
    public Task<CanonProposalIngestionResult> IngestAsync(
        CanonProposalBundle proposal,
        CanonProposalValidationContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validation = validator.Validate(proposal, context);

        if (!validation.IsAccepted)
        {
            return Task.FromResult(new CanonProposalIngestionResult(
                Accepted: false,
                Issues: validation.Issues,
                AcceptedDomains: Array.Empty<Domain>()));
        }

        // RFC-V2-0072: accepted proposals are translated to normal truth events.
        // Event translation mechanics are intentionally deferred.
        return Task.FromResult(new CanonProposalIngestionResult(
            Accepted: true,
            Issues: Array.Empty<CanonProposalValidationIssue>(),
            AcceptedDomains: proposal.AffectedDomains));
    }
}
