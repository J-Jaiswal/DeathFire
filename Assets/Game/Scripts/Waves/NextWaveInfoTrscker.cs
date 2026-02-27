using UnityEngine;
using System.Collections.Generic;

public class NextWaveInfoTracker : MonoBehaviour
{
    public static NextWaveInfoTracker Instance { get; private set; }

    public WaveData NextWave { get; private set; }
    public int TotalEnemies { get; private set; }

    // "Grunt" -> 3, "Sniper" -> 2  (counted from duplicate GOs in the array)
    public Dictionary<string, int> EnemyTypeCounts { get; private set; } = new();

    // Ammo
    public bool FullAmmoRefill { get; private set; }
    public int AmmoRefillAmount { get; private set; }

    // Health
    public bool HealthDrop { get; private set; }
    public float HealthDropChance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetNextWave(WaveData nextWave)
    {
        NextWave = nextWave;
        EnemyTypeCounts.Clear();

        if (nextWave == null)
        {
            TotalEnemies = 0;
            FullAmmoRefill = false;
            AmmoRefillAmount = 0;
            HealthDrop = false;
            HealthDropChance = 0f;
            Debug.Log("[NextWaveInfoTracker] No next wave — this is the last.");
            return;
        }

        // Count duplicates in the flat GameObject array
        TotalEnemies = nextWave.enemies != null ? nextWave.enemies.Length : 0;

        if (nextWave.enemies != null)
        {
            foreach (var go in nextWave.enemies)
            {
                if (go == null) continue;
                string typeName = go.name; // use prefab name as type label

                if (!EnemyTypeCounts.ContainsKey(typeName))
                    EnemyTypeCounts[typeName] = 0;
                EnemyTypeCounts[typeName]++;
            }
        }

        FullAmmoRefill = nextWave.fullAmmoRefill;
        AmmoRefillAmount = nextWave.ammoRefillAmount;
        HealthDrop = nextWave.healthDrop;
        HealthDropChance = nextWave.healthDropChance;

        Debug.Log($"[NextWaveInfoTracker] ✅ '{nextWave.waveName}' | " +
                  $"Enemies: {TotalEnemies} | " +
                  $"Ammo: {(FullAmmoRefill ? "FULL" : AmmoRefillAmount.ToString())} | " +
                  $"Health drop: {(HealthDrop ? $"{HealthDropChance * 100f:F0}%" : "None")}");
    }

    public void Clear() => SetNextWave(null);
}