using UnityEngine;

public class WaveStatsTracker : MonoBehaviour
{
    public static WaveStatsTracker Instance { get; private set; }

    // ── Per-wave stats ───────────────────────────────────────────
    public int Kills { get; private set; }
    public int Headshots { get; private set; }
    public int ShotsFired { get; private set; }
    public int ShotsHit { get; private set; }

    // ── Cumulative (whole run) ───────────────────────────────────
    public int TotalKills { get; private set; }
    public int TotalHeadshots { get; private set; }
    public int TotalShotsFired { get; private set; }
    public int TotalShotsHit { get; private set; }

    public float Accuracy => ShotsFired > 0 ? (float)ShotsHit / ShotsFired * 100f : 0f;
    public float TotalAccuracy => TotalShotsFired > 0 ? (float)TotalShotsHit / TotalShotsFired * 100f : 0f;

    // ── Lifecycle ────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Public API ───────────────────────────────────────────────
    /// <summary>Call once when the run ends (game over / victory).</summary>
    public void CommitRunResults(int finalScore , int waveReached)
    {

        GameResultsData.Score = finalScore;
        GameResultsData.waveReached = waveReached;

        GameResultsData.Kills = TotalKills;
        GameResultsData.Headshots = TotalHeadshots;
        GameResultsData.ShotsFired = TotalShotsFired;
        GameResultsData.ShotsHit = TotalShotsHit;
    }
    /// <summary>Call at the start of each wave to reset per-wave counters.</summary>
    public void ResetForNewWave()
    {
        Kills = 0;
        Headshots = 0;
        ShotsFired = 0;
        ShotsHit = 0;
    }

    /// <summary>Call from Enemy death handler.</summary>
    public void RegisterKill()
    {
        Kills++;
        TotalKills++;
       
    }

    public void RegisterHeadShot()
    {
        Headshots++;
        TotalHeadshots++;
    }

    /// <summary>Call from weapon fire handler.</summary>
    public void RegisterFired()
    {
        ShotsFired++;
        TotalShotsFired++;
     
    }

    public void RegisterHit()
    {
        ShotsHit++;
        TotalShotsHit++;
    }
}