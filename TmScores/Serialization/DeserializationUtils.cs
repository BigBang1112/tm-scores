using System;
using System.Buffers.Binary;
using System.Collections.Generic;
namespace TmScores.Serialization;

internal static class DeserializationUtils
{
    public static DateTimeOffset? GetGzipTimestamp(Stream stream)
    {
        if (!stream.CanSeek)
        {
            return null;
        }

        Span<byte> buffer = stackalloc byte[10];
        stream.ReadExactly(buffer);

        if (buffer[0] != 0x1F || buffer[1] != 0x8B)
        {
            throw new InvalidDataException("Invalid GZip header while reading timestamp.");
        }

        stream.Position -= 10;

        var mtime = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(4, 4));

        if (mtime != 0)
        {
            return DateTimeOffset.FromUnixTimeSeconds(mtime);
        }

        return null;
    }
}
