using KellermanSoftware.CompareNetObjects;

namespace TmScores.Tests;

public class GeneralScoresTests
{
    [Fact]
    public void Deserialize_TMF_Zone()
    {
        var filePath = Path.Combine("Files", "General104085.gz");

        var scores = GeneralScores.Deserialize(filePath);

        Assert.Equal(expected: 5, actual: scores.Version);
        Assert.Equal(expected: "World", actual: scores[0].LeagueName);
        Assert.Equal(expected: 38591, actual: scores[0].Skillpoints.Length);
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
    }

    [Fact]
    public void Deserialize_TMF_Group()
    {
        var filePath = Path.Combine("Files", "General123213.gz");

        var scores = GeneralScores.Deserialize(filePath);

        Assert.Equal(expected: 5, actual: scores.Version);
        Assert.Equal(expected: "$03c1cc Solo", actual: scores[0].LeagueName);
        Assert.Equal(expected: 58, actual: scores[0].Skillpoints.Length);
        Assert.Equal(expected: 10, actual: scores[0].HighScores.Length);
    }

    [Theory]
    [InlineData("General104085.gz")]
    [InlineData("General123213.gz")]
    [InlineData("General100589.gz")]
    [InlineData("General129055.gz")]
    public void Serialization_Equality(string fileName)
    {
        var filePath = Path.Combine("Files", fileName);

        var inputScores = GeneralScores.Deserialize(filePath);

        using var ms = new MemoryStream();
        inputScores.Serialize(ms);
        ms.Position = 0;

        var outputScores = GeneralScores.Deserialize(ms);

        outputScores.ShouldCompare(inputScores, compareConfig: new() { MaxDifferences = 10 });
    }
}
