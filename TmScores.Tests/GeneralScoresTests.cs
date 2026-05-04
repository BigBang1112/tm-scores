namespace TmScores.Tests;

public class GeneralScoresTests
{
    [Fact]
    public void Deserialize_Zone()
    {
        var filePath = Path.Combine("Files", "General104085.gz");

        var scores = GeneralScores.Deserialize(filePath);

        Assert.Equal(expected: 5, actual: scores.Version);
        Assert.Equal(expected: "World", actual: scores[0].LeagueName);
        Assert.Equal(expected: 38591, actual: scores[0].Skillpoints.Length);
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
    }

    [Fact]
    public void Deserialize_Group()
    {
        var filePath = Path.Combine("Files", "General123213.gz");

        var scores = GeneralScores.Deserialize(filePath);

        Assert.Equal(expected: 5, actual: scores.Version);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].LeagueName);
        Assert.Equal(expected: 58, actual: scores[0].Skillpoints.Length);
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
    }
}
