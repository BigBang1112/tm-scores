using System.Collections;
using System.IO.Compression;
using TmScores.Serialization;

namespace TmScores;

public sealed class LadderScores : IScores, ICollection<LadderLeague>
{
    private byte version = 1;
    private List<LadderLeague> leagues = [];

    public byte Version { get => version; set => version = value; }

    public int Count => leagues.Count;

    bool ICollection<LadderLeague>.IsReadOnly => false;

    public static LadderScores Deserialize(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return Deserialize(fs);
    }

    public static LadderScores Deserialize(Stream stream)
    {
        using var gz = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        using var r = new ScoresReader(gz);
        var rw = new ScoresReaderWriter(r);

        var scores = new LadderScores();
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
        using var gz = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
        using var w = new ScoresWriter(gz);
        var rw = new ScoresReaderWriter(w);

        ReadWrite(rw);
    }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.Byte(ref version);
        rw.ListReadableWritable(ref leagues, lengthPrefixAsByte: true);
    }

    public LadderLeague GetLeagueByName(string name)
    {
        return leagues.FirstOrDefault(z => z.Name == name)
            ?? throw new ArgumentException($"No league with name '{name}' found.");
    }

    public LadderLeague this[int index] => leagues[index];
    public LadderLeague this[string name] => GetLeagueByName(name);

    public void Add(LadderLeague item) => leagues.Add(item);
    public void Clear() => leagues.Clear();
    public bool Contains(LadderLeague item) => leagues.Contains(item);
    public void CopyTo(LadderLeague[] array, int arrayIndex) => leagues.CopyTo(array, arrayIndex);
    public bool Remove(LadderLeague item) => leagues.Remove(item);
    public IEnumerator<LadderLeague> GetEnumerator() => leagues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"LadderScores (version {version}, {leagues.Count} leagues)";
    }
}
