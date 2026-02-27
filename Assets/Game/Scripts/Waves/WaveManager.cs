using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaveSpawner))]
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private float delayBeforeFirstWave = 5f;
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
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        waveSpawner = GetComponent<WaveSpawner>();
        if (waveUI == null) Debug.LogWarning("[WaveManager] ⚠️ WaveUIManager not assigned!");
    }

    private void Start()
    {
        StartCoroutine(RunWave(delayBeforeFirstWave, isFirstWave: true));
    }

    public void NotifyEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log($"[WaveManager] 💀 Enemy died | Alive: {enemiesAlive} | Spawning: {isSpawning}");
        CheckWaveCompletion();
    }

    public void TriggerGameOver(string reason = "")
    {
        StopAllCoroutines();
        waveInProgress = false;
        isSpawning = false;
        waveUI?.ShowGameOver(reason);
        Debug.Log($"[WaveManager] 💀 Game Over | {reason}");
    }

    private IEnumerator RunWave(float delay, bool isFirstWave = false, float clearedDelay = 0f)
    {
        if (clearedDelay > 0f)
            yield return new WaitForSeconds(clearedDelay);

        if (currentWaveIndex >= waves.Count)
        {
            waveUI?.ShowVictory();
            yield break;
        }

        WaveData wave = waves[currentWaveIndex];
        int waveNumber = currentWaveIndex + 1;

        if (isFirstWave)
            yield return StartCoroutine(Countdown(delay, wave, waveNumber));
        else
            yield return StartCoroutine(CountdownWithQueueBuild(delay, wave, waveNumber));

        waveInProgress = true;
        isSpawning = true;
        enemiesAlive = 0;

        waveUI?.ShowWaveStarted(waveNumber);

        yield return StartCoroutine(waveSpawner.SpawnWave(wave, () => { enemiesAlive++; }));

        isSpawning = false;
        CheckWaveCompletion();
    }

    private IEnumerator Countdown(float seconds, WaveData wave, int waveNumber)
    {
        int remaining = Mathf.CeilToInt(seconds);
        waveSpawner.PrepareQueue(wave);

        while (remaining > 0)
        {
            waveUI?.ShowCountdown(remaining, waveNumber);
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        // "GET READY!" beat
        waveUI?.ShowCountdown(0, waveNumber);
        yield return new WaitForSeconds(0.6f);
        waveUI?.HideCountdown();
    }

    private IEnumerator CountdownWithQueueBuild(float seconds, WaveData nextWave, int waveNumber)
    {
        int remaining = Mathf.CeilToInt(seconds);

   
        waveUI?.ShowCountdown(remaining, waveNumber);
        yield return new WaitForSeconds(1f);
        remaining--;

        EnemyPool.Instance.BuildWaveQueue(nextWave.enemies);
        EnemyPool.Instance.LogPoolState();

        while (remaining > 0)
        {
            waveUI?.ShowCountdown(remaining, waveNumber);
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        
        waveUI?.ShowCountdown(0, waveNumber);
        yield return new WaitForSeconds(0.6f);
        waveUI?.HideCountdown();
    }

    private void CheckWaveCompletion()
    {
        if (enemiesAlive > 0 || isSpawning || !waveInProgress) return;

        waveInProgress = false;
        waveUI?.ShowWaveCleared(currentWaveIndex + 1);

        currentWaveIndex++;
        
        StartCoroutine(RunWave(delayBetweenWaves, clearedDelay: 2f));
    }
}