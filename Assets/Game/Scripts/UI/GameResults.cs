
public static class GameResultsData
{
    public static int Score;
    public static int Kills;
    public static int Headshots;
    public static int ShotsFired;
    public static int ShotsHit;

    public static float Accuracy =>
        ShotsFired > 0 ? (float)ShotsHit / ShotsFired * 100f : 0f;

    public static float HeadshotPercent =>
        Kills > 0 ? (float)Headshots / Kills * 100f : 0f;

    public static void Reset()
    {
        Score = 0;
        Kills = 0;
        Headshots = 0;
        ShotsFired = 0;
        ShotsHit = 0;
    }
}