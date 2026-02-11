using FantaSim.Schemas.Serialization;

namespace FantaSim.Geosphere.Plate.Kinematics.Serializers;

public static class MessagePackEventRecordSerializer
{
    public const int SchemaVersionV1 = MessagePackTickEventRecordSerializer.SchemaVersionV1;
    public const int HashSizeBytes = MessagePackTickEventRecordSerializer.HashSizeBytes;

    public static byte[] SerializeRecord(
        int schemaVersion,
        long tick,
        byte[] previousHash,
        byte[] hash,
        byte[] eventBytes)
        => MessagePackTickEventRecordSerializer.SerializeRecord(
            schemaVersion,
            tick,
            previousHash,
            hash,
            eventBytes);

    public static byte[] ComputeHashV1(
        int schemaVersion,
        long tick,
        byte[] previousHash,
        byte[] eventBytes)
        => MessagePackTickEventRecordSerializer.ComputeHashV1(
            schemaVersion,
            tick,
            previousHash,
            eventBytes);

    public static EventRecordV1 DeserializeRecord(byte[] recordBytes)
    {
        var record = MessagePackTickEventRecordSerializer.DeserializeRecord(recordBytes);
        return new EventRecordV1(
            record.SchemaVersion,
            record.Tick,
            record.PreviousHash,
            record.Hash,
            record.EventBytes);
    }

    public static bool TryDeserializeRecord(byte[] recordBytes, out EventRecordV1 record)
    {
        if (MessagePackTickEventRecordSerializer.TryDeserializeRecord(recordBytes, out var sharedRecord))
        {
            record = new EventRecordV1(
                sharedRecord.SchemaVersion,
                sharedRecord.Tick,
                sharedRecord.PreviousHash,
                sharedRecord.Hash,
                sharedRecord.EventBytes);
            return true;
        }

        record = default;
        return false;
    }

    public static byte[] GetZeroHash() => MessagePackTickEventRecordSerializer.GetZeroHash();

    public readonly record struct EventRecordV1(
        int SchemaVersion,
        long Tick,
        byte[] PreviousHash,
        byte[] Hash,
        byte[] EventBytes);
}
