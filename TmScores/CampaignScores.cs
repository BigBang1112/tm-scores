using System.Collections;
using System.IO.Compression;
using TmScores.Serialization;

namespace TmScores;

public sealed class CampaignScores : IReadableWritable, ICollection<CampaignLeague>
{
    private byte version = 7;
    private List<CampaignLeague> leagues = [];

    public byte Version { get => version; set => version = value; }

    public int Count => leagues.Count;
    public bool IsReadOnly => false;

    public static CampaignScores Deserialize(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return Deserialize(fs);
    }

    public static CampaignScores Deserialize(Stream stream)
    {
        using var gz = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        using var r = new ScoresReader(gz);
        var rw = new ScoresReaderWriter(r);

        var scores = new CampaignScores();
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

        if (version != 7)
        {
            throw new NotSupportedException("Unsupported version: " + version);
        }

        rw.ListReadableWritable(ref leagues, lengthPrefixAsByte: true);
    }

    public CampaignLeague GetLeagueByName(string name)
    {
        return leagues.FirstOrDefault(z => z.Name == name)
            ?? throw new ArgumentException($"No league with name '{name}' found.");
    }

    public CampaignLeague this[int index] => leagues[index];
    public CampaignLeague this[string name] => GetLeagueByName(name);

    public void Add(CampaignLeague item) => leagues.Add(item);
    public void Clear() => leagues.Clear();
    public bool Contains(CampaignLeague item) => leagues.Contains(item);
    public void CopyTo(CampaignLeague[] array, int arrayIndex) => leagues.CopyTo(array, arrayIndex);
    public bool Remove(CampaignLeague item) => leagues.Remove(item);
    public IEnumerator<CampaignLeague> GetEnumerator() => leagues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"CampaignScores (version {version}, {leagues.Count} leagues)";
    }
}
