namespace FantaSim.App.Bundles.Contracts.Interaction.Commands;

/// <summary>
/// Loosely-typed bridge command for GDScript bundles that can't use generics.
/// Routed by action name in <see cref="GdScriptCommand.Action"/>.
/// </summary>
public sealed record GdScriptCommand(
    Guid CommandId,
    string Action,
    IReadOnlyDictionary<string, object?> Params) : IHudCommand;
