using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


[RequireComponent(typeof(WaveSpawner))]
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private float delayBeforeFirstWave = 3f;
    [SerializeField] private float delayBetweenWaves = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;      
    [SerializeField] private TextMeshProUGUI countdownText;  

    private WaveSpawner waveSpawner;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;
    private bool waveInProgress = false;

    private void Awake()
    {
        waveSpawner = GetComponent<WaveSpawner>();
    }

    private void Start()
    {
        StartCoroutine(RunWave(delayBeforeFirstWave));
    }



    public void NotifyEnemyDeath()
    {
        enemiesAlive--;
        CheckWaveCompletion();
    }

  

    private IEnumerator RunWave(float delay)
    {
        if (currentWaveIndex >= waves.Count)
        {
            SetWaveText("ALL WAVES COMPLETE");
            SetCountdownText("");
            yield break;
        }

        // countdown before wave starts
        yield return StartCoroutine(Countdown(delay));

        // start wave
        WaveData wave = waves[currentWaveIndex];
        waveInProgress = true;
        isSpawning = true;
        enemiesAlive = 0;

        SetWaveText($"WAVE {currentWaveIndex + 1}");
        SetCountdownText("");

        yield return StartCoroutine(waveSpawner.SpawnWave(wave, () => enemiesAlive++));

        isSpawning = false;
        CheckWaveCompletion(); // edge case: enemies died mid-spawn
    }

    private IEnumerator Countdown(float seconds)
    {
        int remaining = Mathf.CeilToInt(seconds);
        while (remaining > 0)
        {
            SetCountdownText(remaining > 0 ? $"Next wave in {remaining}..." : "GET READY!");
            yield return new WaitForSeconds(1f);
            remaining--;
        }
        SetCountdownText("GET READY!");
        yield return new WaitForSeconds(0.5f);
    }

    private void CheckWaveCompletion()
    {
        if (enemiesAlive > 0 || isSpawning || !waveInProgress) return;

        waveInProgress = false;
        SetWaveText($"WAVE {currentWaveIndex + 1} CLEAR!");
        currentWaveIndex++;
        StartCoroutine(RunWave(delayBetweenWaves));
    }

    // ── UI helpers ────────────────────────────────────────────────────────────

    private void SetWaveText(string msg)
    {
        if (waveText != null) waveText.text = msg;
    }

    private void SetCountdownText(string msg)
    {
        if (countdownText != null) countdownText.text = msg;
    }
}