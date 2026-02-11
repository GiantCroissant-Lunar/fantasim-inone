using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Selection;

namespace FantaSim.App.Bundles.Core;

/// <summary>
/// Single-select selection tracker. Publishes <see cref="SelectionChangedEvent"/> via the message bus
/// when the selection changes. Deduplicates — no event if the same item is re-selected.
/// </summary>
public sealed class SelectionService : ISelectionService
{
    private readonly IBundleMessageBus _bus;

    public SelectionService(IBundleMessageBus bus)
    {
        _bus = bus;
    }

    public SelectableRef? Current { get; private set; }

    public void Select(SelectableRef item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Current == item)
            return;

        var previous = Current;
        Current = item;
        _bus.Publish(new SelectionChangedEvent(previous, Current));
    }

    public void Clear()
    {
        if (Current is null)
            return;

        var previous = Current;
        Current = null;
        _bus.Publish(new SelectionChangedEvent(previous, null));
    }
}
