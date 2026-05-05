using TmScores.Serialization;

namespace TmScores;

public interface IScores : IReadableWritable
{
    void Serialize(string fileName);
    void Serialize(Stream stream);
}
