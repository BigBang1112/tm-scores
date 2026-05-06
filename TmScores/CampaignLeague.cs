using TmScores.Serialization;

namespace TmScores;

public sealed class CampaignLeague : IReadableWritable
{
    private string name = string.Empty;
    private CampaignChallengeScores[] challengeScores = [];
    private CampaignMedalLeague[] medalLeagues = [];

    public string Name { get => name; set => name = value; }
    public CampaignChallengeScores[] ChallengeScores { get => challengeScores; set => challengeScores = value; }
    public CampaignMedalLeague[] MedalLeagues { get => medalLeagues; set => medalLeagues = value; }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref name);
        rw.ArrayReadableWritable(ref challengeScores);
        rw.ArrayReadableWritable(ref medalLeagues, lengthPrefixAsByte: true);
    }

    public CampaignMedalLeague GetMedalLeagueByName(string name)
    {
        return medalLeagues.FirstOrDefault(m => m.Name == name)
            ?? throw new ArgumentException($"No league with name '{name}'.");
    }

    public override string ToString()
    {
        return $"CampaignLeague ({name}, {challengeScores.Length} maps, {medalLeagues.Length} medal leagues)";
    }
}
