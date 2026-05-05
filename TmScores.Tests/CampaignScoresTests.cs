using KellermanSoftware.CompareNetObjects;

namespace TmScores.Tests;

public class CampaignScoresTests
{
    [Fact]
    public void Deserialize_Zone_ExpectedValues()
    {
        var filePath = Path.Combine("Files", "UnitedRace104085.gz");

        var scores = CampaignScores.Deserialize(filePath);

        Assert.Equal(expected: 7, actual: scores.Version);
        Assert.Equal(expected: "World|Czech republic|Jihoceský kraj", actual: scores[0].Name);
        Assert.Equal(expected: 147, actual: scores[0].ChallengeScores.Length);
        Assert.Equal(expected: "4kQ_0mHdJtnSdsQty9ZX0W8SDb1", actual: scores[0].ChallengeScores[0].MapUid);

        Assert.Equal(expected: 3, actual: scores[0].ChallengeScores[0].Count);
        Assert.Equal(expected: "World", actual: scores[0].ChallengeScores[0][0].LeagueName);
        Assert.Equal(expected: 2243, actual: scores[0].ChallengeScores[0][0].Skillpoints.Length);
        Assert.Equal(expected: 22060, actual: scores[0].ChallengeScores[0][0].Skillpoints[0].Score);
        Assert.Equal(expected: 1, actual: scores[0].ChallengeScores[0][0].Skillpoints[0].Count);
        Assert.Equal(expected: 10, actual: scores[0].ChallengeScores[0][0].HighScores.Length);
        Assert.Equal(expected: 1, actual: scores[0].ChallengeScores[0][0].HighScores[0].Rank);
        Assert.Equal(expected: 22060, actual: scores[0].ChallengeScores[0][0].HighScores[0].Score);
        Assert.Equal(expected: "jack1998-1998", actual: scores[0].ChallengeScores[0][0].HighScores[0].Login);
        Assert.Equal(expected: "$i$f00ғ$c00ฟ$900๏.$fffRollin $f00¬ $c00Law", actual: scores[0].ChallengeScores[0][0].HighScores[0].Nickname);

        Assert.Equal(expected: 3, actual: scores[0].MedalLeagues.Length);
        Assert.Equal(expected: "World", actual: scores[0].MedalLeagues[0].Name);
        Assert.Single(scores[0].MedalLeagues[0]);
        Assert.Equal(expected: 588, actual: scores[0].MedalLeagues[0][0].Count);
        Assert.Equal(expected: PlayMode.Race, actual: scores[0].MedalLeagues[0][0].PlayMode);
        Assert.Equal(expected: 1607, actual: scores[0].MedalLeagues[0][0][0].Count);
        Assert.Equal(expected: 588, actual: scores[0].MedalLeagues[0][0][0].Score);
    }

    [Fact]
    public void Deserialize_Group_ExpectedValues()
    {
        var filePath = Path.Combine("Files", "UnitedRace123213.gz");

        var scores = CampaignScores.Deserialize(filePath);

        Assert.Equal(expected: 7, actual: scores.Version);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].Name);
        Assert.Equal(expected: 147, actual: scores[0].ChallengeScores.Length);
        Assert.Equal(expected: "4kQ_0mHdJtnSdsQty9ZX0W8SDb1", actual: scores[0].ChallengeScores[0].MapUid);

        Assert.Single(scores[0].ChallengeScores[0]);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].ChallengeScores[0][0].LeagueName);
        Assert.Equal(expected: 24, actual: scores[0].ChallengeScores[0][0].Skillpoints.Length);
        Assert.Equal(expected: 22080, actual: scores[0].ChallengeScores[0][0].Skillpoints[0].Score);
        Assert.Equal(expected: 2, actual: scores[0].ChallengeScores[0][0].Skillpoints[0].Count);
        Assert.Equal(expected: 10, actual: scores[0].ChallengeScores[0][0].HighScores.Length);
        Assert.Equal(expected: 5, actual: scores[0].ChallengeScores[0][0].HighScores[0].Rank);
        Assert.Equal(expected: 22080, actual: scores[0].ChallengeScores[0][0].HighScores[0].Score);
        Assert.Equal(expected: "chris_ri", actual: scores[0].ChallengeScores[0][0].HighScores[0].Login);
        Assert.Equal(expected: "»$fffLuffy", actual: scores[0].ChallengeScores[0][0].HighScores[0].Nickname);

        Assert.Single(scores[0].MedalLeagues);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].MedalLeagues[0].Name);
        Assert.Single(scores[0].MedalLeagues[0]);
        Assert.Equal(expected: 34, actual: scores[0].MedalLeagues[0][0].Count);
        Assert.Equal(expected: PlayMode.Race, actual: scores[0].MedalLeagues[0][0].PlayMode);
        Assert.Equal(expected: 25, actual: scores[0].MedalLeagues[0][0][0].Count);
        Assert.Equal(expected: 588, actual: scores[0].MedalLeagues[0][0][0].Score);
    }

    [Theory]
    [InlineData("UnitedRace104085.gz")]
    [InlineData("UnitedRace123213.gz")]
    [InlineData("TMCanyon132.gz")]
    [InlineData("TMStadium443.gz")]
    [InlineData("TMStadium100589.gz")]
    [InlineData("TMStadium129055.gz")]
    [InlineData("TMValley104088.gz")]
    [InlineData("LTMCanyon443.gz")]
    public void Serialization_Equality(string fileName)
    {
        var filePath = Path.Combine("Files", fileName);

        var inputScores = CampaignScores.Deserialize(filePath);

        using var ms = new MemoryStream();
        inputScores.Serialize(ms);
        ms.Position = 0;

        var outputScores = CampaignScores.Deserialize(ms);

        outputScores.ShouldCompare(inputScores, compareConfig: new() { MaxDifferences = 10 });
    }

    [Fact]
    public void Serialization_Custom_Equality()
    {
        var scores = new CampaignScores
        {
            new CampaignLeague
            {
                Name = "World|Czech republic|Jihoceský kraj",
                ChallengeScores = [
                    new CampaignChallengeScores("4kQ_0mHdJtnSdsQty9ZX0W8SDb1")
                    {
                        new Scores
                        {
                            LeagueName = "World",
                            Skillpoints = [new RecordUnit<int> { Score = 22060, Count = 1 }],
                            HighScores = [new HighScore(1, 22060, "jack1998-1998", "$i$f00ғ$c00ฟ$900๏.$fffRollin $f00¬ $c00Law")]
                        }
                    }
                ],
                MedalLeagues = [
                    new CampaignMedalLeague("World")
                    {
                        new CampaignMedalPlayMode(PlayMode.Race, [new RecordUnit<int>(22060, 1)])
                    }
                ]
            }
        };

        using var ms = new MemoryStream();
        scores.Serialize(ms);
        ms.Position = 0;

        var deserializedScores = CampaignScores.Deserialize(ms);

        Assert.Equal(expected: scores.Version, actual: deserializedScores.Version);
        Assert.Equal(expected: scores[0].Name, actual: deserializedScores[0].Name);
        Assert.Equal(expected: scores[0].ChallengeScores.Length, actual: deserializedScores[0].ChallengeScores.Length);
        Assert.Equal(expected: scores[0].ChallengeScores[0].MapUid, actual: deserializedScores[0].ChallengeScores[0].MapUid);
        Assert.Equal(expected: scores[0].ChallengeScores[0].Count, actual: deserializedScores[0].ChallengeScores[0].Count);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].LeagueName, actual: deserializedScores[0].ChallengeScores[0][0].LeagueName);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].Skillpoints.Length, actual: deserializedScores[0].ChallengeScores[0][0].Skillpoints.Length);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].Skillpoints[0].Score, actual: deserializedScores[0].ChallengeScores[0][0].Skillpoints[0].Score);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].Skillpoints[0].Count, actual: deserializedScores[0].ChallengeScores[0][0].Skillpoints[0].Count);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].HighScores.Length, actual: deserializedScores[0].ChallengeScores[0][0].HighScores.Length);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].HighScores[0].Rank, actual: deserializedScores[0].ChallengeScores[0][0].HighScores[0].Rank);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].HighScores[0].Score, actual: deserializedScores[0].ChallengeScores[0][0].HighScores[0].Score);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].HighScores[0].Login, actual: deserializedScores[0].ChallengeScores[0][0].HighScores[0].Login);
        Assert.Equal(expected: scores[0].ChallengeScores[0][0].HighScores[0].Nickname, actual: deserializedScores[0].ChallengeScores[0][0].HighScores[0].Nickname);
        Assert.Equal(expected: scores[0].MedalLeagues.Length, actual: deserializedScores[0].MedalLeagues.Length);
        Assert.Equal(expected: scores[0].MedalLeagues[0].Name, actual: deserializedScores[0].MedalLeagues[0].Name);
        Assert.Equal(expected: scores[0].MedalLeagues[0].Count, actual: deserializedScores[0].MedalLeagues[0].Count);
        Assert.Equal(expected: scores[0].MedalLeagues[0][0].PlayMode, actual: deserializedScores[0].MedalLeagues[0][0].PlayMode);
        Assert.Equal(expected: scores[0].MedalLeagues[0][0][0].Count, actual: deserializedScores[0].MedalLeagues[0][0][0].Count);
        Assert.Equal(expected: scores[0].MedalLeagues[0][0][0].Score, actual: deserializedScores[0].MedalLeagues[0][0][0].Score);
    }
}
