using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaveSpawner))]
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private float delayBeforeFirstWave = 3f;
    [SerializeField] private float delayBetweenWaves = 5f;

    [Header("UI")]
    [SerializeField] private WaveUIManager waveUI;

    private WaveSpawner waveSpawner;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;
    private bool waveInProgress = false;

    public static WaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[WaveManager] ⚠️ Multiple instances detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        waveSpawner = GetComponent<WaveSpawner>();
        if (waveUI == null) Debug.LogWarning("[WaveManager] ⚠️ WaveUIManager is NOT assigned in Inspector!");
    }

    private void Start()
    {
        Debug.Log($"[WaveManager] 🎮 Start | Waves: {waves.Count} | First wave delay: {delayBeforeFirstWave}s");
        StartCoroutine(RunWave(delayBeforeFirstWave, isFirstWave: true));
    }

    public void NotifyEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log($"[WaveManager] 💀 Enemy died | Alive: <color=red>{enemiesAlive}</color> | Spawning: {isSpawning}");
        CheckWaveCompletion();
    }

    private IEnumerator RunWave(float delay, bool isFirstWave = false)
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("[WaveManager] 🏆 All waves complete!");
            waveUI?.ShowAllWavesCleared();
            yield break;
        }

        WaveData wave = waves[currentWaveIndex];
        Debug.Log($"[WaveManager] ▶️ RunWave() | Index: {currentWaveIndex} | Name: <color=white>{wave.waveName}</color>");

        if (isFirstWave)
        {
            // First wave: build queue then countdown
            yield return StartCoroutine(Countdown(delay, wave));
        }
        else
        {
            // Between waves: build queue DURING countdown
            yield return StartCoroutine(CountdownWithQueueBuild(delay, wave));
        }

        waveInProgress = true;
        isSpawning = true;
        enemiesAlive = 0;

        Debug.Log($"[WaveManager] ⚔️ Wave <color=white>{wave.waveName}</color> STARTING");
        waveUI?.ShowWaveStarted(currentWaveIndex + 1);

        yield return StartCoroutine(waveSpawner.SpawnWave(wave, () =>
        {
            enemiesAlive++;
            Debug.Log($"[WaveManager] ➕ enemiesAlive: <color=yellow>{enemiesAlive}</color>");
        }));

        isSpawning = false;
        Debug.Log($"[WaveManager] 🛑 Spawning done | enemiesAlive: {enemiesAlive}");
        CheckWaveCompletion();
    }

    /// <summary>
    /// First wave countdown — builds queue upfront before timer starts.
    /// </summary>
    private IEnumerator Countdown(float seconds, WaveData wave)
    {
        int remaining = Mathf.CeilToInt(seconds);
        Debug.Log($"[WaveManager] ⏳ Countdown: {remaining}s");

        // Build queue before countdown so first wave is always ready
        waveSpawner.PrepareQueue(wave);

        while (remaining > 0)
        {
            Debug.Log($"[WaveManager] ⏱️ {remaining}...");
            waveUI?.ShowCountdown(remaining);
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        waveUI?.ShowCountdown(0);
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Between waves: builds the next wave queue silently after first tick
    /// so instantiation happens during downtime the player already waits through.
    /// </summary>
    private IEnumerator CountdownWithQueueBuild(float seconds, WaveData nextWave)
    {
        int remaining = Mathf.CeilToInt(seconds);
        Debug.Log($"[WaveManager] ⏳ Countdown with queue build: {remaining}s");

        // Show first tick, let wave-cleared UI settle
        waveUI?.ShowCountdown(remaining);
        yield return new WaitForSeconds(1f);
        remaining--;

        // ✅ Build ordered wave queue silently during downtime
        Debug.Log("[WaveManager] 🔁 Building wave queue...");
        EnemyPool.Instance.BuildWaveQueue(nextWave.enemies);
        EnemyPool.Instance.LogPoolState();
        Debug.Log("[WaveManager] ✅ Queue ready for next wave.");

        while (remaining > 0)
        {
            Debug.Log($"[WaveManager] ⏱️ {remaining}...");
            waveUI?.ShowCountdown(remaining);
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        waveUI?.ShowCountdown(0);
        yield return new WaitForSeconds(0.5f);
    }

    private void CheckWaveCompletion()
    {
        Debug.Log($"[WaveManager] 🔍 Check | alive:{enemiesAlive} spawning:{isSpawning} inProgress:{waveInProgress}");
        if (enemiesAlive > 0 || isSpawning || !waveInProgress) return;

        waveInProgress = false;
        Debug.Log($"[WaveManager] ✅ <color=green>WAVE {currentWaveIndex + 1} CLEARED!</color>");
        waveUI?.ShowWaveCleared(currentWaveIndex + 1);

        currentWaveIndex++;
        StartCoroutine(RunWave(delayBetweenWaves));
    }
}