namespace TmScores;

public sealed record HighScore(
    int Rank, 
    int Score, 
    string Login, 
    string Nickname, 
    string? FilePath = null, 
    string? GhostUrl = null);