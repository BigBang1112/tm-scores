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

    public void WriteRecordsBuffer(RecordUnit<uint>[] array)
    {
        var sizeOfScoreInt = array.Max(x => x.Score) switch
        {
            <= byte.MaxValue => 1,
            <= ushort.MaxValue => 2,
            _ => 4
        };

        var sizeOfCountsInt = array.Max(x => x.Count) switch
        {
            <= byte.MaxValue => 1,
            <= ushort.MaxValue => 2,
            _ => 4
        };

        WriteSizesMask2(sizeOfScoreInt, sizeOfCountsInt);
        Write((byte)0); // CompareAsc if 1? otherwise CompareDesc
        Write(array.Length);

        Span<byte> scoreData = stackalloc byte[array.Length * sizeOfScoreInt];
        Span<byte> countsData = stackalloc byte[array.Length * sizeOfCountsInt];

        for (var i = 0; i < array.Length; i++)
        {
            var scoreBytes = BitConverter.GetBytes(array[i].Score);
            var countBytes = BitConverter.GetBytes(array[i].Count);
            scoreBytes.AsSpan(0, sizeOfScoreInt).CopyTo(scoreData.Slice(i * sizeOfScoreInt, sizeOfScoreInt));
            countBytes.AsSpan(0, sizeOfCountsInt).CopyTo(countsData.Slice(i * sizeOfCountsInt, sizeOfCountsInt));
        }

        Write(scoreData);
        Write(countsData);
    }
}