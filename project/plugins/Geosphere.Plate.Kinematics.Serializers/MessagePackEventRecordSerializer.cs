using System;
using System.Buffers;
using System.Security.Cryptography;
using MessagePack;

namespace FantaSim.Geosphere.Plate.Kinematics.Serializers;

public static class MessagePackEventRecordSerializer
{
    public const int SchemaVersionV1 = 1;
    public const int HashSizeBytes = 32;

    private static readonly byte[] ZeroHash = new byte[HashSizeBytes];

    public static byte[] SerializeRecord(
        int schemaVersion,
        long tick,
        byte[] previousHash,
        byte[] hash,
        byte[] eventBytes)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(previousHash);
        ArgumentNullException.ThrowIfNull(hash);
        ArgumentNullException.ThrowIfNull(eventBytes);
#else
        if (previousHash is null) throw new ArgumentNullException(nameof(previousHash));
        if (hash is null) throw new ArgumentNullException(nameof(hash));
        if (eventBytes is null) throw new ArgumentNullException(nameof(eventBytes));
#endif

        if (previousHash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(previousHash), $"previousHash must be {HashSizeBytes} bytes");
        if (hash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(hash), $"hash must be {HashSizeBytes} bytes");

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

    public static byte[] ComputeHashV1(
        int schemaVersion,
        long tick,
        byte[] previousHash,
        byte[] eventBytes)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(previousHash);
        ArgumentNullException.ThrowIfNull(eventBytes);
#else
        if (previousHash is null) throw new ArgumentNullException(nameof(previousHash));
        if (eventBytes is null) throw new ArgumentNullException(nameof(eventBytes));
#endif

        if (previousHash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(previousHash), $"previousHash must be {HashSizeBytes} bytes");

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(4);
        writer.Write(schemaVersion);
        writer.Write(tick);
        writer.Write(previousHash);
        writer.Write(eventBytes);
        writer.Flush();

#if NET5_0_OR_GREATER
        return SHA256.HashData(buffer.WrittenSpan);
#else
        using (var sha = SHA256.Create())
            return sha.ComputeHash(buffer.WrittenSpan.ToArray());
#endif
    }

    public static EventRecordV1 DeserializeRecord(byte[] recordBytes)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(recordBytes);
#else
        if (recordBytes is null) throw new ArgumentNullException(nameof(recordBytes));
#endif

        var reader = new MessagePackReader(recordBytes);
        var length = reader.ReadArrayHeader();
        if (length != 5)
            throw new InvalidOperationException($"EventRecord must have 5 elements, got {length}");

        var schemaVersion = reader.ReadInt32();
        var tick = reader.ReadInt64();

        var previousHash = ReadFixedHash(ref reader, "previousHash");
        var hash = ReadFixedHash(ref reader, "hash");

        var eventBytes = reader.ReadBytes();
        if (!eventBytes.HasValue)
            throw new InvalidOperationException("EventRecord eventBytes cannot be null");

        return new EventRecordV1(schemaVersion, tick, previousHash, hash, eventBytes.Value.ToArray());
    }

    public static bool TryDeserializeRecord(byte[] recordBytes, out EventRecordV1 record)
    {
        try
        {
            record = DeserializeRecord(recordBytes);
            return true;
        }
        catch
        {
            record = default;
            return false;
        }
    }

    public static byte[] GetZeroHash() => (byte[])ZeroHash.Clone();

    private static byte[] ReadFixedHash(ref MessagePackReader reader, string name)
    {
        var bytes = reader.ReadBytes();
        if (!bytes.HasValue)
            throw new InvalidOperationException($"EventRecord {name} cannot be null");

        var arr = bytes.Value.ToArray();
        if (arr.Length != HashSizeBytes)
            throw new InvalidOperationException($"EventRecord {name} must be {HashSizeBytes} bytes, got {arr.Length}");

        return arr;
    }

    public readonly record struct EventRecordV1(
        int SchemaVersion,
        long Tick,
        byte[] PreviousHash,
        byte[] Hash,
        byte[] EventBytes);
}
