using System.Collections;
using TmScores.Serialization;

namespace TmScores;

public sealed class CampaignMedalPlayMode : IReadableWritable, IReadOnlyCollection<RecordUnit<uint>>
{
    private PlayMode playMode;
    private RecordUnit<uint>[] medals = [];

    public PlayMode PlayMode { get => playMode; set => playMode = value; }

    public int Count => medals.Length;

    public CampaignMedalPlayMode() { }

    public CampaignMedalPlayMode(PlayMode playMode, RecordUnit<uint>[] medals)
    {
        this.playMode = playMode;
        this.medals = medals;
    }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        playMode = (PlayMode)rw.Byte((byte)playMode);
        rw.RecordsBuffer(ref medals);
    }

    public RecordUnit<uint> this[int index] => medals[index];

    public IEnumerator<RecordUnit<uint>> GetEnumerator() => ((IEnumerable<RecordUnit<uint>>)medals).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return $"CampaignMedalPlayMode ({playMode}, {medals.Length} medals)";
    }
}
