using TmScores.Serialization;

namespace TmScores;

public interface IScores : IReadableWritable
{
    /// <summary>
    /// Timestamp of when the scores snapshot was created. This will be <see langword="null"/> if the original data stream was not seekable or if the timestamp was not available.
    /// </summary>
    DateTimeOffset? Timestamp { get; }

    void Serialize(string fileName);
    void Serialize(Stream stream);
}
