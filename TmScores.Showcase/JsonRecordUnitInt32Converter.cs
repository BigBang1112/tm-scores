using System.Text.Json;
using System.Text.Json.Serialization;

namespace TmScores.Showcase;

public sealed class JsonRecordUnitInt32Converter : JsonConverter<RecordUnit<int>>
{
    public override RecordUnit<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        var score = reader.GetInt32();
        reader.Read();
        var count = reader.GetInt32();
        reader.Read();
        return new(score, count);
    }

    public override void Write(Utf8JsonWriter writer, RecordUnit<int> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Score);
        writer.WriteNumberValue(value.Count);
        writer.WriteEndArray();
    }
}
