using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WaveSpawner))]
public class WaveSpawnController : MonoBehaviour
{
    public static WaveSpawnController Instance { get; private set; }

    private WaveSpawner waveSpawner;

    private int enemiesAlive;
    private int enemiesSpawned;
    private bool isSpawning;
    private bool waveInProgress;

    // ── Lifecycle ────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        waveSpawner = GetComponent<WaveSpawner>();
    }

    // ── Public API ───────────────────────────────────────────────

    public void PrepareQueue(WaveData wave) => waveSpawner.PrepareQueue(wave);

    public IEnumerator RunWave(WaveData wave)
    {
        enemiesAlive = 0;
        enemiesSpawned = 0;
        isSpawning = true;
        waveInProgress = true;

        yield return StartCoroutine(waveSpawner.SpawnWave(wave, OnEnemySpawned));

        isSpawning = false;
        CheckCompletion();
    }

    /// <summary>Called by Enemy scripts when they die.</summary>
    public void NotifyEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log($"[WaveSpawnController] Enemy died | Alive: {enemiesAlive} | Spawned: {enemiesSpawned}");
        CheckCompletion();
    }

    // ── Internal ─────────────────────────────────────────────────

    private void OnEnemySpawned()
    {
        enemiesAlive++;
        enemiesSpawned++;
    }

    private void CheckCompletion()
    {
        if (!waveInProgress) return;
        if (isSpawning) return;
        if (enemiesAlive > 0) return;
        if (enemiesSpawned == 0) return;

        waveInProgress = false;
        WaveManager.Instance.OnWaveComplete();
    }
}