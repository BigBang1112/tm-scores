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
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
    }
}
