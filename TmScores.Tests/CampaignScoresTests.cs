namespace TmScores.Tests;

public class CampaignScoresTests
{
    [Fact]
    public void Deserialize_Zone()
    {
        var filePath = Path.Combine("Files", "UnitedRace104085.gz");

        var scores = CampaignScores.Deserialize(filePath);

        Assert.Equal(expected: 7, actual: scores.Version);
        Assert.Equal(expected: "World|Czech republic|Jihoceský kraj", actual: scores[0].Name);
        Assert.Equal(expected: 147, actual: scores[0].ChallengeScores.Length);
        Assert.Equal(expected: 3, actual: scores[0].MedalLeagues.Length);
    }

    [Fact]
    public void Deserialize_Group()
    {
        var filePath = Path.Combine("Files", "UnitedRace123213.gz");

        var scores = CampaignScores.Deserialize(filePath);

        Assert.Equal(expected: 7, actual: scores.Version);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].Name);
        Assert.Equal(expected: 147, actual: scores[0].ChallengeScores.Length);
        Assert.Single(scores[0].MedalLeagues);
    }
}
