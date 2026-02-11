using System.Text.Json;
using FantaSim.App.Bundles.Contracts;
using FantaSim.App.Bundles.Contracts.Interaction.Events;
using FantaSim.App.Bundles.Core;
using FantaSim.App.Bundles.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace FantaSim.App.Bundles.Tests;

public class BundleHostTests
{
    private readonly FakeGodotBundleVfs _vfs = new();
    private readonly BundleServiceRegistry _registry = new();
    private readonly DllExtractor _extractor;
    private readonly FakeBundleSceneHost _sceneHost = new();
    private readonly MessagePipeBundleMessageBus _bus = new();
    private readonly BundleHost _host;

    public BundleHostTests()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"bundle-test-{Guid.NewGuid():N}");
        _extractor = new DllExtractor(tempDir);
        _host = new BundleHost(_vfs, _extractor, _registry, _sceneHost, _bus);
    }

    [Fact]
    public async Task LoadAsync_mounts_pck_and_parses_manifest()
    {
        SetupFakeBundle("test.bundle", "TestBundle.dll");

        var info = await _host.LoadAsync("test.bundle.pck");

        info.Id.Should().Be("test.bundle");
        info.Manifest.EntryAssembly.Should().Be("TestBundle.dll");
        info.Status.Should().Be(BundleStatus.Loaded);
        _vfs.WasMounted("test.bundle.pck").Should().BeTrue();
    }

    [Fact]
    public async Task LoadAsync_duplicate_throws()
    {
        SetupFakeBundle("test.bundle", "TestBundle.dll");

        await _host.LoadAsync("test.bundle.pck");

        var act = () => _host.LoadAsync("test.bundle.pck");
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already loaded*");
    }

    [Fact]
    public async Task UnloadAsync_removes_bundle()
    {
        SetupFakeBundle("test.bundle", "TestBundle.dll");
        await _host.LoadAsync("test.bundle.pck");

        await _host.UnloadAsync("test.bundle");

        _host.LoadedBundles.Should().BeEmpty();
    }

    [Fact]
    public async Task UnloadAsync_nonexistent_throws()
    {
        var act = () => _host.UnloadAsync("nonexistent");
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not loaded*");
    }

    [Fact]
    public async Task UnloadAllAsync_clears_all_bundles()
    {
        SetupFakeBundle("bundle.a", "BundleA.dll");
        SetupFakeBundle("bundle.b", "BundleB.dll");

        await _host.LoadAsync("bundle.a.pck");
        await _host.LoadAsync("bundle.b.pck");

        _host.LoadedBundles.Should().HaveCount(2);

        await _host.UnloadAllAsync();

        _host.LoadedBundles.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_bundle_without_dll_succeeds()
    {
        SetupFakeBundleNoDll("asset.bundle");

        var info = await _host.LoadAsync("asset.bundle.pck");

        info.Id.Should().Be("asset.bundle");
        info.Manifest.EntryAssembly.Should().BeNull();
        info.Status.Should().Be(BundleStatus.Loaded);
    }

    [Fact]
    public async Task UnloadAsync_bundle_without_dll_succeeds()
    {
        SetupFakeBundleNoDll("asset.bundle");
        await _host.LoadAsync("asset.bundle.pck");

        await _host.UnloadAsync("asset.bundle");

        _host.LoadedBundles.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_calls_scene_host_on_loaded()
    {
        SetupFakeBundle("test.bundle", "TestBundle.dll");

        await _host.LoadAsync("test.bundle.pck");

        _sceneHost.LoadedCalls.Should().ContainSingle()
            .Which.BundleId.Should().Be("test.bundle");
    }

    [Fact]
    public async Task UnloadAsync_calls_scene_host_on_unloading()
    {
        SetupFakeBundle("test.bundle", "TestBundle.dll");
        await _host.LoadAsync("test.bundle.pck");

        await _host.UnloadAsync("test.bundle");

        _sceneHost.UnloadingCalls.Should().ContainSingle()
            .Which.Should().Be("test.bundle");
    }

    [Fact]
    public async Task LoadAsync_publishes_BundleLoadedEvent()
    {
        SetupFakeBundleNoDll("test.bundle");
        BundleLoadedEvent? received = null;
        using var sub = _bus.Subscribe<BundleLoadedEvent>(e => received = e);

        await _host.LoadAsync("test.bundle.pck");

        received.Should().NotBeNull();
        received!.BundleId.Should().Be("test.bundle");
        received.Manifest.Id.Should().Be("test.bundle");
    }

    [Fact]
    public async Task UnloadAsync_publishes_BundleUnloadedEvent()
    {
        SetupFakeBundleNoDll("test.bundle");
        await _host.LoadAsync("test.bundle.pck");

        BundleUnloadedEvent? received = null;
        using var sub = _bus.Subscribe<BundleUnloadedEvent>(e => received = e);

        await _host.UnloadAsync("test.bundle");

        received.Should().NotBeNull();
        received!.BundleId.Should().Be("test.bundle");
    }

    [Fact]
    public async Task UnloadAllAsync_publishes_event_for_each_bundle()
    {
        SetupFakeBundleNoDll("bundle.a");
        SetupFakeBundleNoDll("bundle.b");
        await _host.LoadAsync("bundle.a.pck");
        await _host.LoadAsync("bundle.b.pck");

        var received = new List<BundleUnloadedEvent>();
        using var sub = _bus.Subscribe<BundleUnloadedEvent>(e => received.Add(e));

        await _host.UnloadAllAsync();

        received.Should().HaveCount(2);
        received.Select(e => e.BundleId).Should().BeEquivalentTo(["bundle.a", "bundle.b"]);
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
        var testDllBytes = File.ReadAllBytes(typeof(BundleHostTests).Assembly.Location);
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
