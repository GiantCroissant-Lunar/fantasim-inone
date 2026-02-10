using Fantasim.App.Bundles.Core;
using FluentAssertions;
using Xunit;

namespace Fantasim.App.Bundles.Tests;

public class MessagePipeBundleMessageBusTests
{
    public sealed record TestMessage(string Value);

    [Fact]
    public void Publish_and_subscribe_round_trip()
    {
        using var bus = new MessagePipeBundleMessageBus();
        TestMessage? received = null;

        using var sub = bus.Subscribe<TestMessage>(msg => received = msg);
        bus.Publish(new TestMessage("hello"));

        received.Should().NotBeNull();
        received!.Value.Should().Be("hello");
    }

    [Fact]
    public void Dispose_subscription_stops_delivery()
    {
        using var bus = new MessagePipeBundleMessageBus();
        var count = 0;

        var sub = bus.Subscribe<TestMessage>(_ => count++);
        bus.Publish(new TestMessage("one"));
        sub.Dispose();
        bus.Publish(new TestMessage("two"));

        count.Should().Be(1);
    }

    [Fact]
    public void Multiple_subscribers_receive_same_message()
    {
        using var bus = new MessagePipeBundleMessageBus();
        var received1 = new List<string>();
        var received2 = new List<string>();

        using var sub1 = bus.Subscribe<TestMessage>(msg => received1.Add(msg.Value));
        using var sub2 = bus.Subscribe<TestMessage>(msg => received2.Add(msg.Value));

        bus.Publish(new TestMessage("shared"));

        received1.Should().ContainSingle().Which.Should().Be("shared");
        received2.Should().ContainSingle().Which.Should().Be("shared");
    }
}
