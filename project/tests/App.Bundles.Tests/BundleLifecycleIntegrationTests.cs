using System.Text.Json;
using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Core;
using FantaSim.App.Bundles.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace FantaSim.App.Bundles.Tests;

public class BundleLifecycleIntegrationTests
{
    private readonly FakeGodotBundleVfs _vfs = new();
    private readonly BundleServiceRegistry _registry = new();
    private readonly DllExtractor _extractor;
    private readonly FakeBundleSceneHost _sceneHost = new();
    private readonly MessagePipeBundleMessageBus _messageBus = new();
    private readonly BundleHost _host;

    public BundleLifecycleIntegrationTests()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"bundle-integ-{Guid.NewGuid():N}");
        _extractor = new DllExtractor(tempDir);
        _host = new BundleHost(_vfs, _extractor, _registry, _sceneHost, _messageBus);
    }

    [Fact]
    public async Task Full_lifecycle_load_snapshot_unload_reload_snapshot()
    {
        // Arrange — register message bus in registry (mirrors BootstrapShim)
        _registry.Register<IBundleMessageBus>(_messageBus);

        SetupFakeBundle("dll.bundle", "DllBundle.dll");
        SetupFakeBundleNoDll("asset.bundle");

        // --- Phase 1: Load DLL bundle, verify snapshot ---
        var dllInfo = await _host.LoadAsync("dll.bundle.pck");
        var snap1 = _host.CaptureSnapshot();

        snap1.Bundles.Should().HaveCount(1);
        var dllSnap = snap1.Bundles.Single(b => b.Id == "dll.bundle");
        dllSnap.HasAssemblyLoadContext.Should().BeTrue();
        dllSnap.Status.Should().Be(BundleStatus.Loaded);
        dllSnap.TrackedSceneNodes.Should().Contain("/root/dll.bundle");

        // --- Phase 2: Load no-DLL bundle, verify snapshot ---
        await _host.LoadAsync("asset.bundle.pck");
        var snap2 = _host.CaptureSnapshot();

        snap2.Bundles.Should().HaveCount(2);
        var assetSnap = snap2.Bundles.Single(b => b.Id == "asset.bundle");
        assetSnap.HasAssemblyLoadContext.Should().BeFalse();
        assetSnap.TrackedSceneNodes.Should().Contain("/root/asset.bundle");

        // Registered service types should include IBundleMessageBus
        snap2.RegisteredServiceTypes.Should().Contain(typeof(IBundleMessageBus).FullName);

        // MessageBus snapshot should be present
        snap2.MessageBus.Should().NotBeNull();

        // --- Phase 3: Unload DLL bundle, snapshot shows only asset bundle ---
        await _host.UnloadAsync("dll.bundle");
        var snap3 = _host.CaptureSnapshot();

        snap3.Bundles.Should().HaveCount(1);
        snap3.Bundles.Single().Id.Should().Be("asset.bundle");

        // --- Phase 4: Reload asset bundle, verify fresh LoadedAt ---
        var oldLoadedAt = snap3.Bundles.Single().LoadedAt;
        await Task.Delay(10); // ensure time advances
        await _host.ReloadAsync("asset.bundle");
        var snap4 = _host.CaptureSnapshot();

        snap4.Bundles.Should().HaveCount(1);
        snap4.Bundles.Single().Id.Should().Be("asset.bundle");
        snap4.Bundles.Single().LoadedAt.Should().BeAfter(oldLoadedAt);

        // --- Phase 5: UnloadAll, snapshot empty ---
        await _host.UnloadAllAsync();
        var snap5 = _host.CaptureSnapshot();

        snap5.Bundles.Should().BeEmpty();

        // --- Phase 6: MessageBus round-trip ---
        string? received = null;
        using var sub = _messageBus.Subscribe<string>(msg => received = msg);

        // After subscribing, ActiveChannelCount should be >= 1
        var snap6 = _host.CaptureSnapshot();
        snap6.MessageBus!.ActiveChannelCount.Should().BeGreaterThanOrEqualTo(1);

        _messageBus.Publish("hello");
        received.Should().Be("hello");
    }

    private void SetupFakeBundle(string bundleId, string entryAssembly)
    {
        var manifest = new BundleManifest(bundleId, "1.0.0", bundleId, entryAssembly, null, []);
        var manifestJson = JsonSerializer.SerializeToUtf8Bytes(manifest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _vfs.AddFile($"res://bundles/{bundleId}/manifest.json", manifestJson);

        // Use the test assembly itself as the "entry DLL" — it's a valid .NET assembly
        var testDllBytes = File.ReadAllBytes(typeof(BundleLifecycleIntegrationTests).Assembly.Location);
        _vfs.AddFile($"res://bundles/{bundleId}/bin/{entryAssembly}", testDllBytes);
    }

    private void SetupFakeBundleNoDll(string bundleId)
    {
        var manifest = new BundleManifest(bundleId, "1.0.0", bundleId, null, null, []);
        var manifestJson = JsonSerializer.SerializeToUtf8Bytes(manifest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _vfs.AddFile($"res://bundles/{bundleId}/manifest.json", manifestJson);
    }
}
