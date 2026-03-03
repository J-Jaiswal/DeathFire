using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaveSpawnController))]
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private float delayBeforeFirstWave = 5f;
    [SerializeField] private float delayBetweenWaves = 5f;

    [Header("References")]
    [SerializeField] private WaveBetweenPanel betweenPanel;

    public static WaveManager Instance { get; private set; }

    private int currentWaveIndex = 0;
    private WaveSpawnController spawnController;

    public int CurrentWaveNumber => currentWaveIndex + 1;
    public bool HasNextWave => currentWaveIndex + 1 < waves.Count;

    // ── Lifecycle ────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        spawnController = GetComponent<WaveSpawnController>();
    }

    private void OnEnable()
    {
        WaveEventBus.OnWaveStarted += OnWaveStartedHandler;
    }

    private void OnDisable()
    {
        WaveEventBus.OnWaveStarted -= OnWaveStartedHandler;
    }

    private void OnWaveStartedHandler(int waveNumber)
    {
        WaveStatsTracker.Instance?.ResetForNewWave();
        Debug.Log($"[WaveManager] Stats reset for Wave {waveNumber}");
    }

    private void Start()
    {
        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("[WaveManager] No waves assigned!");
            return;
        }

        // Pre-fill next wave info for between-panel on first clear
        if (waves.Count > 1)
            NextWaveInfoTracker.Instance?.SetNextWave(waves[1]);

        StartCoroutine(RunWave(delayBeforeFirstWave, isFirstWave: true));
    }

    // ── Called by WaveSpawnController ────────────────────────────

    public void OnWaveComplete()
    {
        WaveEventBus.RaiseWaveCleared(CurrentWaveNumber);
        StartCoroutine(BetweenWaveSequence());
    }

    public void TriggerGameOver(string reason = "")
    {
        StopAllCoroutines();
        WaveEventBus.RaiseGameOver(reason);
        Debug.Log($"[WaveManager] Game Over: {reason}");
    }

    // ── Wave Flow ────────────────────────────────────────────────


    private IEnumerator RunWave(float delay, bool isFirstWave = false)
    {
        if (currentWaveIndex >= waves.Count)
        {
            WaveEventBus.RaiseVictory();
            yield break;
        }

        WaveData wave = waves[currentWaveIndex];
        AmmoSystem.Instance.ApplyWaveAmmoRefill(wave);

        if (delay > 0f)
        {
            if (isFirstWave)
                yield return StartCoroutine(CountdownSimple(delay, CurrentWaveNumber, wave));
            else
                yield return StartCoroutine(CountdownWithQueueBuild(delay, CurrentWaveNumber, wave));
        }

        WaveEventBus.RaiseWaveStarted(CurrentWaveNumber);
        yield return null; // One frame buffer for UI to settle
        yield return StartCoroutine(spawnController.RunWave(wave));
    }

    private IEnumerator BetweenWaveSequence()
    {
        yield return new WaitForSecondsRealtime(2f); // Unscaled — safe if timeScale is 0

        int completedWave = currentWaveIndex;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            NextWaveInfoTracker.Instance?.Clear();
            WaveEventBus.RaiseVictory();
            yield break;
        }

        WaveData nextWave = waves[currentWaveIndex];

        NextWaveInfoTracker.Instance?.SetNextWave(nextWave);

        EnemyPool.Instance.BuildWaveQueue(nextWave.enemies);
        EnemyPool.Instance.LogPoolState();

        if (betweenPanel != null)
            yield return StartCoroutine(betweenPanel.Show(completedWave + 1, nextWave, delayBetweenWaves));

        StartCoroutine(RunWave(0f));
    }


    // ── Countdowns ───────────────────────────────────────────────

    private IEnumerator CountdownSimple(float seconds, int waveNumber, WaveData wave)
    {
        spawnController.PrepareQueue(wave);
        yield return StartCoroutine(TickCountdown(seconds, waveNumber));
    }

    private IEnumerator CountdownWithQueueBuild(float seconds, int waveNumber, WaveData wave)
    {
        int remaining = Mathf.CeilToInt(seconds);

        // Show first tick, then build pool on frame 2 to avoid hitch
        WaveEventBus.RaiseCountdownTick(remaining, waveNumber);
        yield return new WaitForSeconds(1f);
        remaining--;

        EnemyPool.Instance.BuildWaveQueue(wave.enemies);
        EnemyPool.Instance.LogPoolState();

        yield return StartCoroutine(TickCountdown(remaining, waveNumber));
    }

    private IEnumerator TickCountdown(float seconds, int waveNumber)
    {
        int remaining = Mathf.CeilToInt(seconds);
        while (remaining > 0)
        {
            WaveEventBus.RaiseCountdownTick(remaining, waveNumber);
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        WaveEventBus.RaiseCountdownTick(0, waveNumber); // triggers "Get Ready!"
        yield return new WaitForSeconds(0.6f);
    }
}