namespace FantaSim.Geosphere.Plate.Runtime.Des.Contracts;

/// <summary>
/// Marker interface for a DES driver.
/// </summary>
public interface IDriver;

/// <summary>
/// Marker interface for a DES trigger.
/// </summary>
public interface ITrigger;

/// <summary>
/// A driver that can be executed within the DES loop.
/// </summary>
public interface IExecutableDriver : IDriver
{
    DriverOutput Execute(DesContext context);
}

/// <summary>
/// A typed DES driver with specific output.
/// </summary>
public interface IDesDriver : IDriver
{
    DriverId DriverId { get; }
}

/// <summary>
/// A typed DES trigger with specific output.
/// </summary>
public interface IDesTrigger : ITrigger
{
    TriggerId TriggerId { get; }
}
