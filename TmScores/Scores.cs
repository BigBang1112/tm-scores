using TmScores.Serialization;

namespace TmScores;

public sealed class Scores : IReadableWritable
{
    private string leagueName = string.Empty;
    private RecordUnit<int>[] skillpoints = [];
    private HighScore[] highScores = [];

    public string LeagueName { get => leagueName; set => leagueName = value; }
    public RecordUnit<int>[] Skillpoints { get => skillpoints; set => skillpoints = value; }
    public HighScore[] HighScores { get => highScores; set => highScores = value; }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref leagueName);

        var hasRecordUnits = rw.IsGeneralScores || rw.Boolean(skillpoints.Length > 0, asByte: true);

        if (hasRecordUnits)
        {
            rw.RecordsBuffer(ref skillpoints);
        }

        var (sizeOfRanksInt, sizeOfScoresInt) = rw.Reader?.ReadSizesMask2() ?? (4, 4);

        if (rw.Writer is not null)
        {
            sizeOfRanksInt = highScores.Max(x => x.Rank) switch
            {
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                _ => 4
            };

            sizeOfScoresInt = highScores.Max(x => x.Score) switch
            {
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                _ => 4
            };

            rw.Writer.WriteSizesMask2(sizeOfRanksInt, sizeOfScoresInt);
        }

        var highScoreCount = rw.Int32(highScores.Length);

        if (highScoreCount == 0)
        {
            highScores = [];
            return;
        }

        Span<int> ranks = rw.IntBuffer(highScores.Select(x => x.Rank), highScoreCount, sizeOfRanksInt);
        Span<int> times = rw.IntBuffer(highScores.Select(x => x.Score), highScoreCount, sizeOfScoresInt);

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

        Span<string> filePaths = [];
        Span<string> ghostUrls = [];

        if (rw.Version >= 8)
        {
            filePaths = rw.Writer is not null ? highScores.Select(x => x.FilePath).ToArray() : new string[highScoreCount];
            for (var i = 0; i < highScoreCount; i++)
            {
                filePaths[i] = rw.String(filePaths[i]);
            }

            ghostUrls = rw.Writer is not null ? highScores.Select(x => x.GhostUrl).ToArray() : new string[highScoreCount];
            for (var i = 0; i < highScoreCount; i++)
            {
                ghostUrls[i] = rw.String(ghostUrls[i]);
            }
        }

        if (rw.Reader is not null)
        {
            highScores = new HighScore[highScoreCount];

            for (var i = 0; i < highScoreCount; i++)
            {
                highScores[i] = new(ranks[i], times[i], logins[i], nicknames[i],
                    filePaths.Length > 0 ? filePaths[i] : string.Empty,
                    ghostUrls.Length > 0 ? ghostUrls[i] : string.Empty);
            }
        }
    }

    public override string ToString()
    {
        return $"Scores ({leagueName}, {skillpoints.Length} skillpoints, {highScores.Length} high scores)";
    }
}
