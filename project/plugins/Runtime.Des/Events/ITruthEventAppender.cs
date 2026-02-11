using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;

namespace FantaSim.Geosphere.Plate.Runtime.Des.Events;

public interface ITruthEventAppender
{
    Task<AppendDraftsResult> AppendAsync(
        IReadOnlyList<ITruthEventDraft> drafts,
        AppendOptions options,
        CancellationToken ct = default);
}
