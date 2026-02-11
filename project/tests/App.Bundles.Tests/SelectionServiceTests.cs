using FantaSim.App.Bundles.Contracts.Interaction.Selection;
using FantaSim.App.Bundles.Core;
using FluentAssertions;

namespace FantaSim.App.Bundles.Tests;

public class SelectionServiceTests
{
    private readonly MessagePipeBundleMessageBus _bus = new();
    private readonly SelectionService _sut;

    public SelectionServiceTests()
    {
        _sut = new SelectionService(_bus);
    }

    [Fact]
    public void Initial_selection_is_null()
    {
        _sut.Current.Should().BeNull();
    }

    [Fact]
    public void Select_sets_current()
    {
        var item = new SelectableRef(SelectableRef.KindPlate, "p1");

        _sut.Select(item);

        _sut.Current.Should().Be(item);
    }

    [Fact]
    public void Select_publishes_SelectionChangedEvent()
    {
        SelectionChangedEvent? received = null;
        using var sub = _bus.Subscribe<SelectionChangedEvent>(e => received = e);

        var item = new SelectableRef(SelectableRef.KindPlate, "p1");
        _sut.Select(item);

        received.Should().NotBeNull();
        received!.Previous.Should().BeNull();
        received.Current.Should().Be(item);
    }

    [Fact]
    public void Select_same_item_does_not_publish_event()
    {
        var item = new SelectableRef(SelectableRef.KindPlate, "p1");
        _sut.Select(item);

        var eventCount = 0;
        using var sub = _bus.Subscribe<SelectionChangedEvent>(_ => eventCount++);

        _sut.Select(item);

        eventCount.Should().Be(0);
    }

    [Fact]
    public void Select_different_item_publishes_with_previous()
    {
        var first = new SelectableRef(SelectableRef.KindPlate, "p1");
        var second = new SelectableRef(SelectableRef.KindBoundary, "b1");
        _sut.Select(first);

        SelectionChangedEvent? received = null;
        using var sub = _bus.Subscribe<SelectionChangedEvent>(e => received = e);
        _sut.Select(second);

        received.Should().NotBeNull();
        received!.Previous.Should().Be(first);
        received.Current.Should().Be(second);
    }

    [Fact]
    public void Clear_sets_current_to_null()
    {
        _sut.Select(new SelectableRef(SelectableRef.KindPlate, "p1"));

        _sut.Clear();

        _sut.Current.Should().BeNull();
    }

    [Fact]
    public void Clear_publishes_event_with_null_current()
    {
        var item = new SelectableRef(SelectableRef.KindPlate, "p1");
        _sut.Select(item);

        SelectionChangedEvent? received = null;
        using var sub = _bus.Subscribe<SelectionChangedEvent>(e => received = e);
        _sut.Clear();

        received.Should().NotBeNull();
        received!.Previous.Should().Be(item);
        received.Current.Should().BeNull();
    }

    [Fact]
    public void Clear_when_already_null_does_not_publish_event()
    {
        var eventCount = 0;
        using var sub = _bus.Subscribe<SelectionChangedEvent>(_ => eventCount++);

        _sut.Clear();

        eventCount.Should().Be(0);
    }
}
