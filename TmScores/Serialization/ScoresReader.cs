using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace TmScores.Serialization;

internal sealed class ScoresReader(Stream input, bool leaveOpen = true)
    : BinaryReader(input, encoding, leaveOpen)
{
    private static readonly Encoding encoding = Encoding.UTF8;

    public override bool ReadBoolean()
    {
        return ReadInt32() != 0;
    }
    
    public bool ReadBoolean(bool asByte)
    {
        return asByte
            ? ReadByte() != 0
            : ReadBoolean();
    }

    public override string ReadString()
    {
        return ReadString(ReadInt32());
    }

    public string ReadString(int length)
    {
        return encoding.GetString(ReadBytes(length));
    }

    public T[] ReadArray<T>(int length) where T : struct
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length is negative.");
        }

        if (length == 0)
        {
            return [];
        }

        var lengthInBytes = length * Unsafe.SizeOf<T>();

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        Span<byte> bytes = stackalloc byte[lengthInBytes];
        ReadExactly(bytes);
#else
        var bytes = ReadBytes(lengthInBytes);
#endif
        return MemoryMarshal.Cast<byte, T>(bytes).ToArray();
    }

    public T[] ReadArray<T>(bool lengthPrefixAsByte = false) where T : struct
    {
        return ReadArray<T>(length: lengthPrefixAsByte ? ReadByte() : ReadInt32());
    }
    
    public T[] ReadArray<T>(int length, Func<ScoresReader, T> func)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length is negative.");
        }

        if (length == 0)
        {
            return [];
        }

        var array = new T[length];

        for (var i = 0; i < length; i++)
        {
            array[i] = func(this);
        }

        return array;
    }

    public T[] ReadArray<T>(Func<ScoresReader, T> func, bool lengthPrefixAsByte = false)
    {
        return ReadArray(length: lengthPrefixAsByte ? ReadByte() : ReadInt32(), func);
    }

    public (int, int) ReadSizesMask2()
    {
        var val = ReadByte(); // ArchiveSizesMask2
        return ((val & 3) + 1, (val >> 2) + 1);
    }

    public int[] ReadIntBuffer(int count, int sizeOfInt)
    {
        var array = new int[count];

        for (var i = 0; i < count; i++)
        {
            array[i] = sizeOfInt switch
            {
                1 => ReadByte(),
                2 => ReadInt16(),
                4 => ReadInt32(),
                _ => throw new ArgumentException($"Invalid sizeOfInt: {sizeOfInt}"),
            };
        }

        return array;
    }

    public RecordUnit<int>[] ReadRecordsBuffer()
    {
        var (sizeOfScoreInt, sizeOfCountsInt) = ReadSizesMask2();

        _ = ReadByte(); // CompareAsc if 1? otherwise CompareDesc

        // SRecordUnit array

        var recordUnitCount = ReadInt32();

        Span<byte> scoreData = ReadBytes(recordUnitCount * sizeOfScoreInt);
        Span<byte> countsData = ReadBytes(recordUnitCount * sizeOfCountsInt);

        var array = new RecordUnit<int>[recordUnitCount];

        for (var i = 0; i < recordUnitCount; i++)
        {
            var scoreSlice = scoreData.Slice(i * sizeOfScoreInt, sizeOfScoreInt);
            var countSlice = countsData.Slice(i * sizeOfCountsInt, sizeOfCountsInt);

            var score = sizeOfScoreInt switch
            {
                1 => scoreSlice[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(scoreSlice),
                4 => BinaryPrimitives.ReadInt32LittleEndian(scoreSlice),
                _ => throw new FormatException($"Invalid score byte size: {sizeOfScoreInt}")
            };

            var count = sizeOfCountsInt switch
            {
                1 => countsData[i],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(countSlice),
                4 => BinaryPrimitives.ReadInt32LittleEndian(countSlice),
                _ => throw new FormatException($"Invalid count byte size: {sizeOfCountsInt}")
            };

            array[i] = new(score, count);
        }

        return array;
    }
}
