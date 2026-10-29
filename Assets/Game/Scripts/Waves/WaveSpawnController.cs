using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaveSpawner))]
public class WaveSpawnController : MonoBehaviour
{

    private WaveSpawner waveSpawner;

    private int enemiesAlive;
    private int enemiesSpawned;
    private bool isSpawning;
    public bool WaveInProgress { get; private set; }
    private bool completionNotified;

    // Track registered enemies to prevent orphan death calls
    private readonly HashSet<GameObject> registeredEnemies = new HashSet<GameObject>();
    public static WaveSpawnController Instance { get; private set; }

   

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        waveSpawner = GetComponent<WaveSpawner>();
    }

    // Public API ------------------->

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
        //Debug.Log($"[WaveSpawnController] Spawning done | Alive: {enemiesAlive} | Spawned: {enemiesSpawned}");
        CheckCompletion();

        // Safety watchdog: if all registered enemies are gone but wave never completed
        // (e.g. enemy returned to pool without calling NotifyEnemyDeath), force-complete.
        StartCoroutine(WatchdogCheck());

        yield return new WaitUntil(() => !WaveInProgress);
    }

    /// <summary>Force-completes the wave — call this as a last resort (e.g. dev console).</summary>
    public void ForceComplete()
    {
        if (!WaveInProgress) return;
        Debug.LogWarning("[WaveSpawnController] ForceComplete called — forcing wave completion.");
        registeredEnemies.Clear();
        enemiesAlive = 0;
        isSpawning = false;
        CheckCompletion();
    }

    private IEnumerator WatchdogCheck()
    {
        // Wait a short grace period so legitimate deaths can resolve first.
        yield return new WaitForSeconds(2f);

        while (WaveInProgress && !completionNotified)
        {
            // If every registered enemy has been returned to pool (not active)
            // the HashSet will be empty and alive count should be 0.
            bool allGone = registeredEnemies.Count == 0 && enemiesAlive <= 0 && !isSpawning && enemiesSpawned > 0;
            if (allGone)
            {
                Debug.LogWarning("[WaveSpawnController] Watchdog: wave stuck — forcing completion.");
                ForceComplete();
                yield break;
            }

            // Remove any enemies that have been deactivated (returned to pool) without proper notification.
            var toRemove = new System.Collections.Generic.List<GameObject>();
            foreach (var e in registeredEnemies)
            {
                if (e == null || !e.activeInHierarchy)
                    toRemove.Add(e);
            }
            if (toRemove.Count > 0)
            {
                Debug.LogWarning($"[WaveSpawnController] Watchdog: found {toRemove.Count} silent-dead enemy(ies) — cleaning up.");
                foreach (var e in toRemove)
                {
                    registeredEnemies.Remove(e);
                    enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
                }
                CheckCompletion();
            }

            yield return new WaitForSeconds(1f);
        }
    }


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

    // Internal ------------->

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