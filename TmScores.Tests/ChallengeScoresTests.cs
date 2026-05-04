namespace TmScores.Tests;

public class ChallengeScoresTests
{
    [Fact]
    public void Deserialize()
    {
        var filePath = Path.Combine("Files", "BeySZdnfuSh4nHY5xztiXLmlrXe.gz");

        var scores = ChallengeScores.Deserialize(filePath);
    }
}
