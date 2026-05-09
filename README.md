# TmScores

[![Nuget](https://img.shields.io/nuget/v/TmScores?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/TmScores/)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/BigBang1112/tm-scores?include_prereleases&style=for-the-badge&logo=github)](https://github.com/BigBang1112/tm-scores/releases)

A .NET library for reading and writing Trackmania master server score files.

Score files are cached leaderboards and medal ranks used between TrackMania Forever and ManiaPlanet 3 versions of Trackmania (2008-2015).

## Scores types

| Type | Description |
|---|---|
| `CampaignScores` | Per-zone or per-group campaign scores, including per-map challenge scores and medal leagues |
| `ChallengeScores` | Per-league scores for an individual map |
| `GeneralScores` | Overall skillpoints and high score leaderboards per league |
| `LadderScores` | Ladder rankings and player counts per league |

## Usage

Install the NuGet package:

```
dotnet add package TmScores
```

### Deserialize

All scores types expose a static `Deserialize` method that accepts a file path or stream. Files are expected to be gzip-compressed (the standard stored format).

**Campaign scores:**

```cs
using TmScores;

var scores = CampaignScores.Deserialize("UnitedRace104085.gz");
```

**Challenge scores (single map):**

```cs
var scores = ChallengeScores.Deserialize("BeySZdnfuSh4nHY5xztiXLmlrXe.gz");
```

**General scores:**

```cs
var scores = GeneralScores.Deserialize("General104085.gz");
```

**Ladder scores:**

```cs
var scores = LadderScores.Deserialize("Multi104085.gz");
```

### Deserialize raw (uncompressed)

Use `DeserializeRaw` to read from an uncompressed stream:

```cs
var scores = CampaignScores.DeserializeRaw("UnitedRace104085");
```

### Serialize

All scores types support round-trip serialization back to the compressed binary format:

```cs
using var ms = new MemoryStream();
scores.Serialize(ms);
```

Or to a file:

```cs
scores.Serialize("output.gz");
```

Use `SerializeRaw` to write uncompressed.

### Build scores from scratch

All types support collection initializer syntax:

```cs
var scores = new CampaignScores
{
    new CampaignLeague
    {
        Name = "World|Czech republic|Jihoceský kraj",
        ChallengeScores =
        [
            new CampaignChallengeScores("4kQ_0mHdJtnSdsQty9ZX0W8SDb1")
            {
                new Scores
                {
                    LeagueName = "World",
                    Skillpoints = [new RecordUnit<int>(22060, 1)],
                    HighScores = [new HighScore(1, 22060, "jack1998-1998", "$i$f00ғ$c00ฟ$900๏.$fffRollin $f00¬ $c00Law")]
                }
            }
        ],
        MedalLeagues =
        [
            new CampaignMedalLeague("World")
            {
                new CampaignMedalPlayMode(PlayMode.Race, [new RecordUnit<int>(22060, 1)])
            }
        ]
    }
};
```

```cs
var scores = new LadderScores
{
    new LadderLeague { Name = "World", PlayerCount = 9782572, Points = [(1, 100), (2, 90)] },
    new LadderLeague { Name = "World|Czech republic", PlayerCount = 385442, Points = [(1, 80), (2, 70)] }
};
```

```cs
var scores = new ChallengeScores
{
    new Scores
    {
        LeagueName = "World",
        Skillpoints = [new RecordUnit<int>(1112, 1)],
        HighScores = [new HighScore(1, 1112, "bigbang1112", "BigBang1112")]
    }
};
```

### Timestamp

When deserializing from a gzip stream, the modification timestamp embedded in the gzip header is exposed via the `Timestamp` property:

```cs
var scores = CampaignScores.Deserialize("UnitedRace104085.gz");
Console.WriteLine(scores.Timestamp); // DateTimeOffset, or null if unavailable
```

## Data model

```
CampaignScores                  (ICollection<CampaignLeague>)
└── CampaignLeague
    ├── ChallengeScores[]       (CampaignChallengeScores per map)
    │   └── Scores[]            (per league)
    │       ├── Skillpoints     (RecordUnit<int>[])
    │       └── HighScores      (HighScore[])
    └── MedalLeagues[]          (CampaignMedalLeague per league)
        └── CampaignMedalPlayMode[] (per PlayMode)
            └── RecordUnit<int>[]

GeneralScores                   (ICollection<Scores>)
└── Scores
    ├── Skillpoints             (RecordUnit<int>[])
    └── HighScores              (HighScore[])

ChallengeScores                 (ICollection<Scores>)
└── Scores
    ├── Skillpoints             (RecordUnit<int>[])
    └── HighScores              (HighScore[])

LadderScores                    (ICollection<LadderLeague>)
└── LadderLeague
    ├── PlayerCount             (int)
    └── Points                  ((int Rank, int Points)[])
```

### Key types

| Type | Description |
|---|---|
| `RecordUnit<T>` | A score value (`Score`) paired with a player count (`Count`) |
| `HighScore` | A leaderboard entry: `Rank`, `Score`, `Login`, `Nickname`, optional `FilePath` and `GhostUrl` |
| `PlayMode` | `Race`, `Puzzle`, `Platform`, `Stunts` |

## Showcase

Simple scores viewer and editor is available [here](https://bigbang1112.github.io/tm-scores/).