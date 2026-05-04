using System.Diagnostics.CodeAnalysis;

namespace TmScores.Serialization;

public sealed class ScoresReaderWriter
{
    internal ScoresReader? Reader { get; }
    internal ScoresWriter? Writer { get; }

    internal bool IsGeneralScores { get; set; }
    internal int Version { get; set; }

    public ScoresReaderWriter(Stream stream, bool writing, bool leaveOpen = true)
    {
        if (writing)
        {
            Writer = new ScoresWriter(stream, leaveOpen);
        }
        else
        {
            Reader = new ScoresReader(stream, leaveOpen);
        }
    }

    internal ScoresReaderWriter(ScoresReader reader) => Reader = reader;

    internal ScoresReaderWriter(ScoresWriter writer) => Writer = writer;

    public byte Byte(byte value = default)
    {
        value = Reader?.ReadByte() ?? value;
        Writer?.Write(value);
        return value;
    }

    public void Byte(ref byte value) => value = Byte(value);

    public bool Boolean(bool value = default)
    {
        value = Reader?.ReadBoolean() ?? value;
        Writer?.Write(value);
        return value;
    }

    public void Boolean(ref bool value) => value = Boolean(value);

    public string String(string value = "")
    {
        value = Reader?.ReadString() ?? value;
        Writer?.Write(value);
        return value;
    }

    public void String(ref string value) => value = String(value);

    public int Int32(int value = default)
    {
        value = Reader?.ReadInt32() ?? value;
        Writer?.Write(value);
        return value;
    }

    public void Int32(ref int value) => value = Int32(value);

    public T Object<T>(T? value, Action<ScoresReaderWriter, T> action) where T : new()
    {
        if (Reader is not null)
        {
            var result = new T();
            action(this, result);
            return result;
        }

        if (Writer is not null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null when writing an object.");
            }

            action(this, value);
        }

        return value!;
    }

    public T[]? Array<T>(T[]? value, bool lengthPrefixAsByte = false) where T : struct
    {
        value = Reader?.ReadArray<T>(lengthPrefixAsByte) ?? value;
        Writer?.WriteArray(value, lengthPrefixAsByte);
        return value;
    }

    public void Array<T>([NotNullIfNotNull(nameof(value))] ref T[]? value, bool lengthPrefixAsByte = false) where T : struct
        => value = Array(value, lengthPrefixAsByte);

    public T[]? Array<T>(T[]? value, Action<ScoresReaderWriter, T> action, bool lengthPrefixAsByte = false) where T : new()
    {
        if (Reader is not null)
        {
            var result = new T[lengthPrefixAsByte ? Reader.ReadByte() : Reader.ReadInt32()];

            for (var i = 0; i < result.Length; i++)
            {
                var item = new T();
                action(this, item);
                result[i] = item;
            }

            return result;
        }

        if (Writer is not null)
        {
            var length = value?.Length ?? 0;

            if (lengthPrefixAsByte)
            {
                Writer.Write((byte)length);
            }
            else
            {
                Writer.Write(length);
            }

            if (value is not null)
            {
                foreach (var item in value)
                {
                    action(this, item);
                }
            }
        }

        return value;
    }

    public T[]? ArrayReadableWritable<T>(T[]? value, bool lengthPrefixAsByte = false) where T : IReadableWritable, new()
    {
        return Array(value, (rw, item) => item.ReadWrite(rw), lengthPrefixAsByte);
    }

    public void ArrayReadableWritable<T>([NotNullIfNotNull(nameof(value))] ref T[]? value, bool lengthPrefixAsByte = false) where T : IReadableWritable, new()
        => value = ArrayReadableWritable(value, lengthPrefixAsByte);

    public List<T>? List<T>(List<T>? value, Action<ScoresReaderWriter, T> action, bool lengthPrefixAsByte = false) where T : new()
    {
        if (Reader is not null)
        {
            var length = lengthPrefixAsByte ? Reader.ReadByte() : Reader.ReadInt32();
            var result = new List<T>(length);

            for (var i = 0; i < length; i++)
            {
                var item = new T();
                action(this, item);
                result.Add(item);
            }

            return result;
        }

        if (Writer is not null)
        {
            var length = value?.Count ?? 0;

            if (lengthPrefixAsByte)
            {
                Writer.Write((byte)length);
            }
            else
            {
                Writer.Write(length);
            }

            if (value is not null)
            {
                foreach (var item in value)
                {
                    action(this, item);
                }
            }
        }

        return value;
    }

    public List<T>? ListReadableWritable<T>(List<T>? value, bool lengthPrefixAsByte = false) where T : IReadableWritable, new()
    {
        return List(value, (rw, item) => item.ReadWrite(rw), lengthPrefixAsByte);
    }

    public void ListReadableWritable<T>([NotNullIfNotNull(nameof(value))] ref List<T>? value, bool lengthPrefixAsByte = false) where T : IReadableWritable, new()
        => value = ListReadableWritable(value, lengthPrefixAsByte);

    public int[] IntBuffer(IEnumerable<int> value, int length, int sizeOfInt)
    {
        if (Reader is not null)
        {
            return Reader.ReadIntBuffer(length, sizeOfInt);
        }

        Writer?.WriteIntBuffer(value, sizeOfInt);
        return value.ToArray();
    }

    public RecordUnit<int>[] RecordsBuffer(RecordUnit<int>[] value)
    {
        value = Reader?.ReadRecordsBuffer() ?? value;
        Writer?.WriteRecordsBuffer(value);
        return value;
    }

    public void RecordsBuffer([NotNullIfNotNull(nameof(value))] ref RecordUnit<int>[] value) => value = RecordsBuffer(value);
}
