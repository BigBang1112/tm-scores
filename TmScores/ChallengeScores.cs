using System.Collections;
using System.IO.Compression;
using TmScores.Serialization;

namespace TmScores;

public sealed class ChallengeScores : IReadableWritable, ICollection<Scores>
{
    private List<Scores> challengeScores = [];

    public int Count => challengeScores.Count;
    public bool IsReadOnly => false;

    public static ChallengeScores Deserialize(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return Deserialize(fs);
    }

    public static ChallengeScores Deserialize(Stream stream)
    {
        using var gz = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        using var r = new ScoresReader(gz);
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
        using var w = new ScoresWriter(gz);
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
