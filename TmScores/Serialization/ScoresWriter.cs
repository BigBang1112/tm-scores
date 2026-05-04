using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace TmScores.Serialization;

internal sealed class ScoresWriter(Stream output, bool leaveOpen = true)
    : BinaryWriter(output, encoding, leaveOpen)
{
    private static readonly Encoding encoding = Encoding.UTF8;

    public override void Write(bool value)
    {
        Write(value ? 1 : 0);
    }

    public void Write(bool value, bool asByte)
    {
        if (asByte)
        {
            Write((byte)(value ? 1 : 0));
        }
        else
        {
            Write(value);
        }
    }

    public override void Write(string value)
    {
        var bytes = encoding.GetBytes(value);
        Write(bytes.Length);
        Write(bytes);
    }

    public void WriteArray<T>(T[]? array, bool lengthPrefixAsByte = false) where T : struct
    {
        var length = array?.Length ?? 0;

        if (lengthPrefixAsByte)
        {
            Write((byte)length);
        }
        else
        {
            Write(length);
        }

        if (array is not null)
        {
            WriteArray(array, length);
        }
    }

    public void WriteArray<T>(T[] array, int length) where T : struct
    {
        Write(MemoryMarshal.AsBytes(array.AsSpan(0, length)));
    }

    public void WriteArray<T>(T[] array, Action<ScoresWriter, T> action, bool lengthPrefixAsByte = false)
    {
        if (lengthPrefixAsByte)
        {
            Write((byte)array.Length);
        }
        else
        {
            Write(array.Length);
        }

        foreach (var item in array)
        {
            action(this, item);
        }
    }

    public void WriteSizesMask2(int sizeOfScoreInt, int sizeOfCountsInt)
    {
        if (sizeOfScoreInt < 1 || sizeOfScoreInt > 4 || sizeOfCountsInt < 1 || sizeOfCountsInt > 4)
        {
            throw new ArgumentException("sizeOfScoreInt and sizeOfCountsInt must be between 1 and 4.");
        }

        var val = (byte)(((sizeOfScoreInt - 1) & 3) | (((sizeOfCountsInt - 1) & 3) << 2));
        Write(val);
    }

    public void WriteIntBuffer(IEnumerable<int> enumerable, int sizeOfInt)
    {
        foreach (var value in enumerable)
        {
            switch (sizeOfInt)
            {
                case 1:
                    Write((byte)value);
                    break;
                case 2:
                    Write((short)value);
                    break;
                case 4:
                    Write(value);
                    break;
                default:
                    throw new ArgumentException($"Invalid sizeOfInt: {sizeOfInt}");
            }
        }
    }

    public void WriteRecordsBuffer(RecordUnit<int>[] array)
    {
        if (array.Length == 0)
        {
            WriteSizesMask2(1, 1);
            Write((byte)0);
            Write(0);
            return;
        }

        var sizeOfScoreInt = array.Max(x => (uint)x.Score) switch
        {
            <= byte.MaxValue => 1,
            <= ushort.MaxValue => 2,
            _ => 4
        };

        var sizeOfCountsInt = array.Max(x => (uint)x.Count) switch
        {
            <= byte.MaxValue => 1,
            <= ushort.MaxValue => 2,
            _ => 4
        };

        WriteSizesMask2(sizeOfScoreInt, sizeOfCountsInt);
        Write((byte)0); // CompareAsc if 1? otherwise CompareDesc
        Write(array.Length);

        Span<byte> scoreSpan = new byte[array.Length * sizeOfScoreInt];
        Span<byte> countSpan = new byte[array.Length * sizeOfCountsInt];
        
        Span<byte> temp4Bytes = stackalloc byte[4];

        for (var i = 0; i < array.Length; i++)
        {
            BinaryPrimitives.WriteInt32LittleEndian(temp4Bytes, array[i].Score);
            temp4Bytes.Slice(0, sizeOfScoreInt).CopyTo(scoreSpan.Slice(i * sizeOfScoreInt, sizeOfScoreInt));

            BinaryPrimitives.WriteUInt32LittleEndian(temp4Bytes, (uint)array[i].Count);
            temp4Bytes.Slice(0, sizeOfCountsInt).CopyTo(countSpan.Slice(i * sizeOfCountsInt, sizeOfCountsInt));
        }

        Write(scoreSpan);
        Write(countSpan);
    }
}