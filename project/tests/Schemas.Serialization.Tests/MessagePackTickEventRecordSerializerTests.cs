using System.Buffers;
using FluentAssertions;
using FantaSim.Schemas.Serialization;
using MessagePack;
using Xunit;

namespace Schemas.Serialization.Tests;

public sealed class MessagePackTickEventRecordSerializerTests
{
    [Fact]
    public void SerializeDeserialize_RoundTripsRecord()
    {
        const int schemaVersion = MessagePackTickEventRecordSerializer.SchemaVersionV1;
        const long tick = 42;
        var previousHash = BuildHash(1);
        var eventBytes = new byte[] { 7, 8, 9 };
        var hash = MessagePackTickEventRecordSerializer.ComputeHashV1(schemaVersion, tick, previousHash, eventBytes);

        var bytes = MessagePackTickEventRecordSerializer.SerializeRecord(schemaVersion, tick, previousHash, hash, eventBytes);
        var record = MessagePackTickEventRecordSerializer.DeserializeRecord(bytes);

        record.SchemaVersion.Should().Be(schemaVersion);
        record.Tick.Should().Be(tick);
        record.PreviousHash.Should().Equal(previousHash);
        record.Hash.Should().Equal(hash);
        record.EventBytes.Should().Equal(eventBytes);
    }

    [Fact]
    public void ComputeHashV1_IsDeterministic()
    {
        var previousHash = BuildHash(3);
        var eventBytes = new byte[] { 1, 2, 3, 4 };

        var first = MessagePackTickEventRecordSerializer.ComputeHashV1(1, 10, previousHash, eventBytes);
        var second = MessagePackTickEventRecordSerializer.ComputeHashV1(1, 10, previousHash, eventBytes);

        first.Should().Equal(second);
    }

    [Fact]
    public void ComputeHashV1_ChangesWhenPayloadChanges()
    {
        var previousHash = BuildHash(5);
        var hashA = MessagePackTickEventRecordSerializer.ComputeHashV1(1, 10, previousHash, new byte[] { 1, 2, 3 });
        var hashB = MessagePackTickEventRecordSerializer.ComputeHashV1(1, 10, previousHash, new byte[] { 1, 2, 4 });

        hashA.Should().NotEqual(hashB);
    }

    [Fact]
    public void TryDeserializeRecord_ReturnsFalseForInvalidRecordShape()
    {
        var invalid = new byte[] { 0x91, 0x01 }; // [1]

        var ok = MessagePackTickEventRecordSerializer.TryDeserializeRecord(invalid, out var record);

        ok.Should().BeFalse();
        record.SchemaVersion.Should().Be(0);
        record.Tick.Should().Be(0);
        record.PreviousHash.Should().BeNull();
        record.Hash.Should().BeNull();
        record.EventBytes.Should().BeNull();
    }

    [Fact]
    public void DeserializeRecord_ThrowsWhenHashLengthIsInvalid()
    {
        var previousHash = BuildHash(1);
        var invalidHash = new byte[] { 1, 2, 3 };
        var eventBytes = new byte[] { 9 };
        var bytes = BuildRecordBytes(1, 4, previousHash, invalidHash, eventBytes);

        var act = () => MessagePackTickEventRecordSerializer.DeserializeRecord(bytes);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*hash must be 32 bytes*");
    }

    [Fact]
    public void GetZeroHash_ReturnsDefensiveCopy()
    {
        var first = MessagePackTickEventRecordSerializer.GetZeroHash();
        var second = MessagePackTickEventRecordSerializer.GetZeroHash();

        first.Length.Should().Be(MessagePackTickEventRecordSerializer.HashSizeBytes);
        second.Length.Should().Be(MessagePackTickEventRecordSerializer.HashSizeBytes);
        first[0] = 123;
        second[0].Should().Be(0);
    }

    private static byte[] BuildHash(byte start)
    {
        var hash = new byte[MessagePackTickEventRecordSerializer.HashSizeBytes];
        for (var i = 0; i < hash.Length; i++)
            hash[i] = (byte)(start + i);
        return hash;
    }

    private static byte[] BuildRecordBytes(int schemaVersion, long tick, byte[] previousHash, byte[] hash, byte[] eventBytes)
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(5);
        writer.Write(schemaVersion);
        writer.Write(tick);
        writer.Write(previousHash);
        writer.Write(hash);
        writer.Write(eventBytes);
        writer.Flush();
        return buffer.WrittenMemory.ToArray();
    }
}
