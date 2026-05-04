using System.Collections;
using TmScores.Serialization;

namespace TmScores;

public sealed class CampaignMedalLeague : IReadableWritable, ICollection<CampaignMedalPlayMode>
{
    private string name = string.Empty;
    private List<CampaignMedalPlayMode> playModes = [];

    public string Name { get => name; set => name = value; }

    public int Count => playModes.Count;
    public bool IsReadOnly => false;

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref name);
        rw.ListReadableWritable(ref playModes, lengthPrefixAsByte: true);
    }

    public CampaignMedalPlayMode GetMedalsByPlayMode(PlayMode playMode)
    {
        return playModes.FirstOrDefault(x => x.PlayMode == playMode)
            ?? throw new ArgumentException($"No play mode with name '{playMode}' found.");
    }

    public CampaignMedalPlayMode this[int index] => playModes[index];
    public CampaignMedalPlayMode this[PlayMode playMode] => GetMedalsByPlayMode(playMode);

    public void Add(CampaignMedalPlayMode item) => playModes.Add(item);
    public void Clear() => playModes.Clear();
    public bool Contains(CampaignMedalPlayMode item) => playModes.Contains(item);
    public void CopyTo(CampaignMedalPlayMode[] array, int arrayIndex) => playModes.CopyTo(array, arrayIndex);
    public bool Remove(CampaignMedalPlayMode item) => playModes.Remove(item);
    public IEnumerator<CampaignMedalPlayMode> GetEnumerator() => playModes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"CampaignMedalLeague ({name}, {playModes.Count} play modes)";
    }
}