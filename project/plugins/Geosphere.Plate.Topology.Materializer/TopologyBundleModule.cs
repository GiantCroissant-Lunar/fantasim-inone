using System;
using FantaSim.App.Bundles.Contracts;
using FantaSim.Geosphere.Plate.Topology.Contracts.Derived;
using FantaSim.Geosphere.Plate.Topology.Contracts.Events;
using ServiceArchi.Contracts;

namespace FantaSim.Geosphere.Plate.Topology.Materializer;

/// <summary>
/// Bundle service module for registering Topology services with the host registry.
/// This is invoked AFTER persistence modules have initialized IKeyValueStore.
/// </summary>
public sealed class TopologyBundleModule : IBundleServiceModule
{
    public void Register(IRegistry registry)
    {
        if (registry.IsRegistered<IPlateTopologyMaterializationService>())
        {
            return;
        }

        var eventStore = registry.TryGet<ITopologyEventStore>();
        if (eventStore == null)
        {
            throw new InvalidOperationException(
                "ITopologyEventStore not registered. Ensure TopologyPersistenceModule has run.");
        }

        var materializer = new PlateTopologyMaterializer(eventStore);
        registry.Register<IPlateTopologyMaterializationService>(materializer);
    }

    public void Deregister(IRegistry registry)
    {
        var materializer = registry.TryGet<IPlateTopologyMaterializationService>();
        if (materializer != null)
        {
            registry.Unregister<IPlateTopologyMaterializationService>(materializer);
        }
    }
}
