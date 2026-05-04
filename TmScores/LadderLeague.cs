using TmScores.Serialization;

namespace TmScores;

public sealed record LadderLeague : IReadableWritable
{
    private string name = string.Empty;
    private int playerCount;
    private (int, int)[] points = [];

    public string Name { get => name; set => name = value; }
    public int PlayerCount { get => playerCount; set => playerCount = value; }
    public (int Rank, int Points)[] Points { get => points; set => points = value; }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref name);
        rw.Int32(ref playerCount);

        var length = rw.Int32(points.Length);

        if (rw.Reader is not null)
        {
            var ranks = rw.Reader.ReadArray<int>(length);
            points = ranks.Zip(rw.Reader.ReadArray<int>(length)).ToArray();
        }

        if (rw.Writer is not null)
        {
            rw.Writer.WriteArray(points.Select(p => p.Item1).ToArray(), length);
            rw.Writer.WriteArray(points.Select(p => p.Item2).ToArray(), length);
        }
    }

    public override string ToString()
    {
        return $"LadderLeague ({name}, {playerCount} players, {points.Length} ranks)";
    }
}
