using System.Collections;
using TmScores.Serialization;

namespace TmScores;

public sealed class CampaignChallengeScores : IReadableWritable, ICollection<Scores>
{
    private string mapUid = string.Empty;
    private List<Scores> challengeScores = [];

    public string MapUid { get => mapUid; set => mapUid = value; }

    public int Count => challengeScores.Count;
    public bool IsReadOnly => false;

    public CampaignChallengeScores() { }

    public CampaignChallengeScores(string mapUid)
    {
        this.mapUid = mapUid;
    }

    public void ReadWrite(ScoresReaderWriter rw)
    {
        rw.String(ref mapUid);

        // CGameChallengeScores
        rw.ListReadableWritable(ref challengeScores, lengthPrefixAsByte: true);
    }

    public Scores GetChallengeScoresByLeagueName(string leagueName)
    {
        return challengeScores.FirstOrDefault(x => x.LeagueName == leagueName)
            ?? throw new ArgumentException($"No league with name '{leagueName}' found.");
    }

    public Scores this[int index] => challengeScores[index];
    public Scores this[string leagueName] => GetChallengeScoresByLeagueName(leagueName);

    public void Add(Scores item) => challengeScores.Add(item);
    public void Clear() => challengeScores.Clear();
    public bool Contains(Scores item) => challengeScores.Contains(item);
    public void CopyTo(Scores[] array, int arrayIndex) => challengeScores.CopyTo(array, arrayIndex);
    public bool Remove(Scores item) => challengeScores.Remove(item);
    public IEnumerator<Scores> GetEnumerator() => challengeScores.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"CampaignChallengeScores ({mapUid}, {challengeScores.Count} leagues)";
    }
}