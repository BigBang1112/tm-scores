using TmScores.Serialization;

namespace TmScores;

public sealed class Scores : IReadableWritable
{
    private string leagueName = string.Empty;
    private RecordUnit<uint>[] skillpoints = [];
    private HighScore[] highScores = [];

    public string LeagueName { get => leagueName; set => leagueName = value; }
    public RecordUnit<uint>[] Skillpoints { get => skillpoints; set => skillpoints = value; }
    public HighScore[] HighScores { get => highScores; set => highScores = value; }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref leagueName);

        var hasRecordUnits = rw.IsGeneralScores || rw.Boolean(skillpoints.Length > 0, asByte: true);

        if (hasRecordUnits)
        {
            rw.RecordsBuffer(ref skillpoints);
        }

        var (sizeOfRanksInt, sizeOfTimesInt) = rw.Reader?.ReadSizesMask2() ?? (4, 4);

        if (rw.Writer is not null)
        {
            sizeOfRanksInt = highScores.Max(x => x.Rank) switch
            {
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                _ => 4
            };

            sizeOfTimesInt = highScores.Max(x => x.Time.TotalMilliseconds) switch
            {
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                _ => 4
            };
        }

        var highScoreCount = rw.Int32(highScores.Length);

        Span<int> ranks = rw.IntBuffer(highScores.Select(x => x.Rank), highScoreCount, sizeOfRanksInt);
        Span<int> times = rw.IntBuffer(highScores.Select(x => x.Time.TotalMilliseconds), highScoreCount, sizeOfTimesInt);

        Span<string> logins = rw.Writer is not null ? highScores.Select(x => x.Login).ToArray() : new string[highScoreCount];
        for (var i = 0; i < highScoreCount; i++)
        {
            logins[i] = rw.String(logins[i]);
        }

        Span<string> nicknames = rw.Writer is not null ? highScores.Select(x => x.Nickname).ToArray() : new string[highScoreCount];
        for (var i = 0; i < highScoreCount; i++)
        {
            nicknames[i] = rw.String(nicknames[i]);
        }

        if (rw.Reader is not null)
        {
            highScores = new HighScore[highScoreCount];

            for (var i = 0; i < highScoreCount; i++)
            {
                highScores[i] = new(ranks[i], times[i], logins[i], nicknames[i]);
            }
        }
    }

    public override string ToString()
    {
        return $"Scores ({leagueName}, {skillpoints.Length} skillpoints, {highScores.Length} high scores)";
    }
}
