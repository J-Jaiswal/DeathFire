using UnityEngine;

public class WaveStatsTracker : MonoBehaviour
{
    public static WaveStatsTracker Instance { get; private set; }

    public int Kills { get; private set; }
    public int Headshots { get; private set; }
    public int ShotsFired { get; private set; }
    public int ShotsHit { get; private set; }

    public float Accuracy => ShotsFired > 0 ? (float)ShotsHit / ShotsFired * 100f : 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ResetForNewWave()
    {
        Kills = Headshots = ShotsFired = ShotsHit = 0;
    }

    public void RegisterKill(bool isHeadshot)
    {
        Kills++;
        if (isHeadshot) Headshots++;
    }

    public void RegisterShot(bool hit)
    {
        ShotsFired++;
        if (hit) ShotsHit++;
    }
}