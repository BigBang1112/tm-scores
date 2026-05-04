namespace TmScores.Tests;

public class LadderScoresTests
{
    [Fact]
    public void Deserialize()
    {
        var filePath = Path.Combine("Files", "Multi104085.gz");

        var scores = LadderScores.Deserialize(filePath);

        Assert.Equal(expected: 1, actual: scores.Version);
        Assert.Equal(expected: 3, actual: scores.Count);
        Assert.Equal(expected: "World", actual: scores[0].Name);
        Assert.Equal(expected: 9782572, actual: scores[0].PlayerCount);
        Assert.Equal(expected: "World|Czech republic", actual: scores[1].Name);
        Assert.Equal(expected: 385442, actual: scores[1].PlayerCount);
        Assert.Equal(expected: "World|Czech republic|Jihoceský kraj", actual: scores[2].Name);
        Assert.Equal(expected: 21550, actual: scores[2].PlayerCount);
    }

    [Fact]
    public void Serialize()
    {
        var scores = new LadderScores
        {
            new LadderLeague
            {
                Name = "World",
                PlayerCount = 9782572,
                Points = [(1, 100), (2, 90)]
            },
            new LadderLeague
            {
                Name = "World|Czech republic",
                PlayerCount = 385442,
                Points = [(1, 80), (2, 70)]
            },
            new LadderLeague
            {
                Name = "World|Czech republic|Jihoceský kraj",
                PlayerCount = 21550,
                Points = [(1, 60), (2, 50)]
            }
        };

        using var ms = new MemoryStream();
        scores.Serialize(ms);

        ms.Position = 0;

        var deserializedLadderScores = LadderScores.Deserialize(ms);

        Assert.Equal(scores.Version, deserializedLadderScores.Version);
        Assert.Equal(scores.Count, deserializedLadderScores.Count);

        for (var i = 0; i < scores.Count; i++)
        {
            Assert.Equal(scores[i].Name, deserializedLadderScores[i].Name);
            Assert.Equal(scores[i].PlayerCount, deserializedLadderScores[i].PlayerCount);
            Assert.Equal(scores[i].Points.Length, deserializedLadderScores[i].Points.Length);

            for (var j = 0; j < scores[i].Points.Length; j++)
            {
                Assert.Equal(scores[i].Points[j], deserializedLadderScores[i].Points[j]);
            }
        }
    }
}
