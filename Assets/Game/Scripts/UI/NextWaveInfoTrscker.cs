using UnityEngine;
using System.Collections.Generic;

public class NextWaveInfoTracker : MonoBehaviour
{
    public static NextWaveInfoTracker Instance { get; private set; }

    public WaveData NextWave { get; private set; }

    public string EnemyDetails;
    public int TotalEnemies { get; private set; }
    public bool FullAmmoRefill { get; private set; }
    public int AmmoRefillAmount { get; private set; }
   


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetNextWave(WaveData nextWave)
    {
        NextWave = nextWave;

        if (nextWave == null)
        {
            TotalEnemies = 0;
            FullAmmoRefill = false;
            AmmoRefillAmount = 0;
            
            Debug.Log("[NextWaveInfoTracker] No next wave — this is the last.");
            return;
        }

        TotalEnemies = nextWave.enemies != null ? nextWave.enemies.Length : 0;
        FullAmmoRefill = nextWave.fullAmmoRefill;
        AmmoRefillAmount = nextWave.ammoRefillAmount;    
        EnemyDetails = nextWave.enemyDetails;
       
    }

    public void Clear() => SetNextWave(null);
}