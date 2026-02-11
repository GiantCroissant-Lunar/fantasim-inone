using System;
using System.Buffers;
using System.Security.Cryptography;
using MessagePack;

namespace FantaSim.Geosphere.Plate.Topology.Serializers;

/// <summary>
/// Hash-chain record serializer for meta/governance events.
/// Record format: [schemaVersion:int, previousHash:bin32, hash:bin32, eventBytes:bin]
/// </summary>
public static class MessagePackMetaEventRecordSerializer
{
    public const int SchemaVersionV1 = 1;
    public const int HashSizeBytes = 32;

    private static readonly byte[] ZeroHash = new byte[HashSizeBytes];

    public static byte[] SerializeRecord(int schemaVersion, byte[] previousHash, byte[] hash, byte[] eventBytes)
    {
        if (previousHash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(previousHash), $"previousHash must be {HashSizeBytes} bytes");
        if (hash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(hash), $"hash must be {HashSizeBytes} bytes");

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(4);
        writer.Write(schemaVersion);
        writer.Write(previousHash);
        writer.Write(hash);
        writer.Write(eventBytes);
        writer.Flush();
        return buffer.WrittenMemory.ToArray();
    }

    public static byte[] ComputeHashV1(int schemaVersion, byte[] previousHash, byte[] eventBytes)
    {
        if (previousHash.Length != HashSizeBytes)
            throw new ArgumentOutOfRangeException(nameof(previousHash), $"previousHash must be {HashSizeBytes} bytes");

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(3);
        writer.Write(schemaVersion);
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

    public static bool TryDeserializeRecord(byte[] recordBytes, out EventRecordV1 record)
    {
        try
        {
            var reader = new MessagePackReader(recordBytes);
            var length = reader.ReadArrayHeader();
            if (length != 4)
            {
                record = default;
                return false;
            }

            var schemaVersion = reader.ReadInt32();
            var previousHash = ReadFixedHash(ref reader, "previousHash");
            var hash = ReadFixedHash(ref reader, "hash");
            var eventBytes = reader.ReadBytes();
            if (!eventBytes.HasValue)
            {
                record = default;
                return false;
            }

            record = new EventRecordV1(schemaVersion, previousHash, hash, eventBytes.Value.ToArray());
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
            throw new InvalidOperationException($"MetaEventRecord {name} cannot be null");

        var arr = bytes.Value.ToArray();
        if (arr.Length != HashSizeBytes)
            throw new InvalidOperationException($"MetaEventRecord {name} must be {HashSizeBytes} bytes, got {arr.Length}");

        return arr;
    }

    public readonly record struct EventRecordV1(
        int SchemaVersion,
        byte[] PreviousHash,
        byte[] Hash,
        byte[] EventBytes);
}
