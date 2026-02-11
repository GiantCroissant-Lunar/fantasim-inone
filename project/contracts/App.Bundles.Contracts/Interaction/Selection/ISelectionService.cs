namespace FantaSim.App.Bundles.Contracts.Interaction.Selection;

/// <summary>
/// Tracks the currently selected entity. Single-select; multi-select deferred.
/// </summary>
public interface ISelectionService
{
    SelectableRef? Current { get; }
    void Select(SelectableRef item);
    void Clear();
}
