using System.Collections;
using System.IO.Compression;
using TmScores.Serialization;

namespace TmScores;

public sealed class ChallengeScores : IScores, ICollection<Scores>
{
    private List<Scores> challengeScores = [];

    public DateTimeOffset? Timestamp { get; }

    public int Count => challengeScores.Count;

    bool ICollection<Scores>.IsReadOnly => false;

    public ChallengeScores() { }

    private ChallengeScores(DateTimeOffset? timestamp)
    {
        Timestamp = timestamp;
    }

    public static ChallengeScores Deserialize(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return Deserialize(fs);
    }

    public static ChallengeScores Deserialize(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var timestamp = DeserializationUtils.GetGzipTimestamp(stream);

        using var gz = new GZipStream(stream, CompressionLevel.SmallestSize, leaveOpen: true);
        using var r = new ScoresReader(gz);
        var rw = new ScoresReaderWriter(r);

        var scores = new ChallengeScores(timestamp);
        scores.ReadWrite(rw);
        return scores;
    }

    public static ChallengeScores DeserializeRaw(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return DeserializeRaw(fs);
    }

    public static ChallengeScores DeserializeRaw(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var r = new ScoresReader(stream);
        var rw = new ScoresReaderWriter(r);

        var scores = new ChallengeScores();
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
        var rw = new ScoresReaderWriter(w);

        ReadWrite(rw);
    }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.ListReadableWritable(ref challengeScores, lengthPrefixAsByte: true);
    }

    public Scores GetLeagueByName(string name)
    {
        return challengeScores.FirstOrDefault(x => x.LeagueName == name)
            ?? throw new ArgumentException($"No league with name '{name}' found.");
    }

    public Scores this[int index] => challengeScores[index];
    public Scores this[string name] => GetLeagueByName(name);

    public void Add(Scores item) => challengeScores.Add(item);
    public void Clear() => challengeScores.Clear();
    public bool Contains(Scores item) => challengeScores.Contains(item);
    public void CopyTo(Scores[] array, int arrayIndex) => challengeScores.CopyTo(array, arrayIndex);
    public bool Remove(Scores item) => challengeScores.Remove(item);
    public IEnumerator<Scores> GetEnumerator() => challengeScores.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"ChallengeScores ({challengeScores.Count} leagues)";
    }
}
