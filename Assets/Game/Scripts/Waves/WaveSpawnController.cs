using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaveSpawner))]
public class WaveSpawnController : MonoBehaviour
{
    public static WaveSpawnController Instance { get; private set; }

    private WaveSpawner waveSpawner;

    private int enemiesAlive;
    private int enemiesSpawned;
    private bool isSpawning;
    public bool WaveInProgress { get; private set; }
    private bool completionNotified;

    // Track registered enemies to prevent orphan death calls
    private readonly HashSet<GameObject> registeredEnemies = new HashSet<GameObject>();

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
        WaveInProgress = true;
        completionNotified = false;
        registeredEnemies.Clear();

        yield return StartCoroutine(waveSpawner.SpawnWave(wave, OnEnemySpawned));

        isSpawning = false;
        Debug.Log($"[WaveSpawnController] Spawning done | Alive: {enemiesAlive} | Spawned: {enemiesSpawned}");
        CheckCompletion();

        yield return new WaitUntil(() => !WaveInProgress);
    }

    /// <summary>
    /// Called by Enemy scripts when they die.
    /// Pass the enemy's GameObject so orphan calls are ignored.
    /// </summary>
    public void NotifyEnemyDeath(GameObject enemy)
    {
        // Ignore deaths from enemies not registered in this wave
        if (!registeredEnemies.Remove(enemy))
        {
            Debug.LogWarning($"[WaveSpawnController] Ignored unregistered death: {enemy.name}");
            return;
        }

        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        Debug.Log($"[WaveSpawnController] Enemy died: {enemy.name} | Alive: {enemiesAlive} | Spawned: {enemiesSpawned}");
        CheckCompletion();
    }

    // ── Internal ─────────────────────────────────────────────────

    // Called by WaveSpawner after GetNext — receives the spawned GameObject
    private void OnEnemySpawned(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("[WaveSpawnController] Spawned enemy was null — skipping registration");
            return;
        }
        registeredEnemies.Add(enemy);
        enemiesAlive++;
        enemiesSpawned++;
        Debug.Log($"[WaveSpawnController] Registered: {enemy.name} | Alive: {enemiesAlive}");
    }

    private void CheckCompletion()
    {
        if (!WaveInProgress) return;
        if (isSpawning) return;
        if (enemiesAlive > 0) return;
        if (enemiesSpawned == 0) return;
        if (completionNotified) return;

        completionNotified = true;
        WaveInProgress = false;
        StartCoroutine(NotifyCompleteNextFrame());
    }

    private IEnumerator NotifyCompleteNextFrame()
    {
        yield return null;
        WaveManager.Instance.OnWaveComplete();
    }
}