using System.Collections;
using System.IO.Compression;
using TmScores.Serialization;

namespace TmScores;

public sealed class GeneralScores : IScores, ICollection<Scores>
{
    private byte version = 5;
    private List<Scores> leagues = [];

    public byte Version { get => version; set => version = value; }

    public DateTimeOffset? Timestamp { get; }

    public int Count => leagues.Count;

    bool ICollection<Scores>.IsReadOnly => false;

    public GeneralScores() { }

    private GeneralScores(DateTimeOffset? timestamp)
    {
        Timestamp = timestamp;
    }

    public static GeneralScores Deserialize(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return Deserialize(fs);
    }

    public static GeneralScores Deserialize(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var timestamp = DeserializationUtils.GetGzipTimestamp(stream);

        using var gz = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        using var r = new ScoresReader(gz);
        var rw = new ScoresReaderWriter(r)
        {
            IsGeneralScores = true
        };

        var scores = new GeneralScores(timestamp);
        scores.ReadWrite(rw);
        return scores;
    }

    public static GeneralScores DeserializeRaw(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return DeserializeRaw(fs);
    }

    public static GeneralScores DeserializeRaw(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var r = new ScoresReader(stream);
        var rw = new ScoresReaderWriter(r)
        {
            IsGeneralScores = true
        };

        var scores = new GeneralScores();
        scores.ReadWrite(rw);
        return scores;
    }

    public void Serialize(string fileName)
    {
        using var fs = File.Create(fileName);
        Serialize(fs);
    }

    public void Serialize(Stream stream)
    {
        using var gz = new GZipStream(stream, CompressionLevel.SmallestSize, leaveOpen: true);
        SerializeRaw(gz);
    }

    public void SerializeRaw(string fileName)
    {
        using var fs = File.Create(fileName);
        SerializeRaw(fs);
    }

    public void SerializeRaw(Stream stream)
    {
        using var w = new ScoresWriter(stream);
        var rw = new ScoresReaderWriter(w)
        {
            IsGeneralScores = true
        };

        ReadWrite(rw);
    }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.Byte(ref version);

        if (version != 5)
        {
            throw new NotSupportedException("Unsupported version: " + version);
        }

        rw.ListReadableWritable(ref leagues, lengthPrefixAsByte: true);
    }

    public Scores GetLeagueByName(string name)
    {
        return leagues.FirstOrDefault(x => x.LeagueName == name)
            ?? throw new ArgumentException($"No league with name '{name}' found.");
    }

    public Scores this[int index] => leagues[index];
    public Scores this[string name] => GetLeagueByName(name);

    public void Add(Scores item) => leagues.Add(item);
    public void Clear() => leagues.Clear();
    public bool Contains(Scores item) => leagues.Contains(item);
    public void CopyTo(Scores[] array, int arrayIndex) => leagues.CopyTo(array, arrayIndex);
    public bool Remove(Scores item) => leagues.Remove(item);
    public IEnumerator<Scores> GetEnumerator() => leagues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"GeneralScores (version {version}, {leagues.Count} leagues)";
    }
}
