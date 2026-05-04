using KellermanSoftware.CompareNetObjects;

namespace TmScores.Tests;

public class ChallengeScoresTests
{
    [Fact]
    public void Deserialize()
    {
        var filePath = Path.Combine("Files", "BeySZdnfuSh4nHY5xztiXLmlrXe.gz");

        var scores = ChallengeScores.Deserialize(filePath);

        Assert.Equal(expected: "World", actual: scores[0].LeagueName);
        Assert.Equal(expected: 1770, actual: scores[0].Skillpoints.Length);
        Assert.Equal(expected: 23770, actual: scores[0].Skillpoints[0].Score);
        Assert.Equal(expected: -2, actual: scores[0].Skillpoints[^1].Score);
        Assert.Equal(expected: 1, actual: scores[0].Skillpoints[0].Count);
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
        Assert.Equal(expected: 1, actual: scores[0].HighScores[0].Rank);
        Assert.Equal(expected: "fwo_axell", actual: scores[0].HighScores[0].Login);
        Assert.Equal(expected: "$i$FC0ה$FE5ѕ$FF9с$000¬ $FFFAxell$FE0. $000Law", actual: scores[0].HighScores[0].Nickname);
        Assert.Equal(expected: 23770, actual: scores[0].HighScores[0].Score);
    }

    [Theory]
    [InlineData("BeySZdnfuSh4nHY5xztiXLmlrXe.gz")]
    public void Serialization_Equality(string fileName)
    {
        var filePath = Path.Combine("Files", fileName);

        var inputScores = ChallengeScores.Deserialize(filePath);

        using var ms = new MemoryStream();
        inputScores.Serialize(ms);
        ms.Position = 0;

        var outputScores = ChallengeScores.Deserialize(ms);

        inputScores.ShouldCompare(outputScores, compareConfig: new() { MaxDifferences = 10 });
    }

    [Fact]
    public void Serialize()
    {
        var scores = new ChallengeScores
        {
            new Scores
            {
                LeagueName = "World",
                Skillpoints = [new RecordUnit<int> { Score = 1112, Count = 1 }],
                HighScores = [new HighScore(1, 1112, "bigbang1112", "$i$FC0ה$FE5ѕ$FF9с$000¬ $FFFAxell$FE0. $000Law")]
            }
        };

        using var ms = new MemoryStream();
        scores.Serialize(ms);
        ms.Position = 0;

        var deserializedScores = ChallengeScores.Deserialize(ms);

        Assert.Equal(scores[0].LeagueName, deserializedScores[0].LeagueName);
        Assert.Equal(scores[0].Skillpoints.Length, deserializedScores[0].Skillpoints.Length);
        Assert.Equal(scores[0].Skillpoints[0].Score, deserializedScores[0].Skillpoints[0].Score);
        Assert.Equal(scores[0].Skillpoints[0].Count, deserializedScores[0].Skillpoints[0].Count);
        Assert.Equal(scores[0].HighScores.Length, deserializedScores[0].HighScores.Length);
        Assert.Equal(scores[0].HighScores[0].Rank, deserializedScores[0].HighScores[0].Rank);
        Assert.Equal(scores[0].HighScores[0].Login, deserializedScores[0].HighScores[0].Login);
        Assert.Equal(scores[0].HighScores[0].Nickname, deserializedScores[0].HighScores[0].Nickname);
        Assert.Equal(scores[0].HighScores[0].Score, deserializedScores[0].HighScores[0].Score);
    }
}
