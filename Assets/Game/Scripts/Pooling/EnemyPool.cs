using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private Dictionary<int, GameObject> prefabRegistry = new();
    private HashSet<GameObject> activeEnemies = new();
    private Queue<GameObject> waveQueue = new();

    // ✅ Holds dead enemies as inactive — destroyed only at next BuildWaveQueue
    private List<GameObject> deadEnemies = new();

    public int QueueCount => waveQueue.Count;

    void Awake()
    {
        Instance = this;
        Debug.Log("[EnemyPool] ✅ Initialized.");
    }

    public void BuildWaveQueue(GameObject[] enemies)
    {
        // ✅ Clean up previous wave's dead enemies now (during countdown, not gameplay)
        foreach (var obj in deadEnemies)
            Destroy(obj);
        deadEnemies.Clear();

        // ✅ Clean up any unspawned objects left in queue from previous wave
        foreach (var obj in waveQueue)
            Destroy(obj);
        waveQueue.Clear();

        // ✅ Destroy any still-active leftovers (edge case: wave ended with alive enemies)
        foreach (var obj in activeEnemies)
        {
            Debug.Log($"[EnemyPool] ⚠️ Destroying active leftover: <color=cyan>{obj.name}</color>");
            Destroy(obj);
        }
        activeEnemies.Clear();

        Debug.Log("[EnemyPool] 🔨 Building wave queue in spawn order...");

        foreach (var prefab in enemies)
        {
            if (prefab == null) continue;
            GameObject obj = CreateNew(prefab);
            waveQueue.Enqueue(obj);
            Debug.Log($"[EnemyPool] ➕ Queued: <color=cyan>{prefab.name}</color>");
        }

        Debug.Log($"[EnemyPool] ✅ Wave queue ready | Total: <color=yellow>{waveQueue.Count}</color>");
    }

    public GameObject GetNext(Vector3 position, Quaternion rotation)
    {
        if (waveQueue.Count == 0)
        {
            Debug.LogError("[EnemyPool] ❌ Wave queue is empty!");
            return null;
        }

        GameObject obj = waveQueue.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        activeEnemies.Add(obj);

        Debug.Log($"[EnemyPool] ✅ <color=green>SPAWNED</color>: <color=cyan>{obj.name}</color> | Remaining in queue: <color=yellow>{waveQueue.Count}</color>");
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (!activeEnemies.Contains(obj))
        {
            Debug.LogWarning($"[EnemyPool] ⚠️ {obj.name} not found in active set. Destroying.");
            Destroy(obj);
            return;
        }

        // ✅ Just deactivate during gameplay — no Destroy hitch
        obj.SetActive(false);
        activeEnemies.Remove(obj);
        deadEnemies.Add(obj); // ✅ Queued for cleanup at next countdown

        Debug.Log($"[EnemyPool] 🔄 <color=orange>RETURNED</color>: <color=cyan>{obj.name}</color> | Dead pool: <color=yellow>{deadEnemies.Count}</color> | Active: <color=yellow>{activeEnemies.Count}</color>");
        //WaveManager.Instance.NotifyEnemyDeath();

        //WaveStatsTracker.Instance?.RegisterKill(isHeadshot);
        //WaveStatsTracker.Instance?.RegisterKill(isHeadshot);
        WaveSpawnController.Instance?.NotifyEnemyDeath(gameObject);
    }

    private GameObject CreateNew(GameObject prefab)
    {
        if (!prefabRegistry.ContainsKey(prefab.GetInstanceID()))
            prefabRegistry[prefab.GetInstanceID()] = prefab;

        GameObject obj = Instantiate(prefab);
        obj.name = prefab.name;
        var tag = obj.AddComponent<PooledObject>();
        tag.PrefabID = prefab.GetInstanceID();
        obj.SetActive(false);
        Debug.Log($"[EnemyPool] 🔨 Created: <color=cyan>{obj.name}</color> (inactive in queue)");
        return obj;
    }

    public void LogPoolState()
    {
        Debug.Log("──────── [EnemyPool] POOL STATE ────────");
        Debug.Log($"  Queued for spawn: <color=yellow>{waveQueue.Count}</color>");
        Debug.Log($"  Active in scene:  <color=red>{activeEnemies.Count}</color>");
        Debug.Log($"  Dead (pending cleanup): <color=grey>{deadEnemies.Count}</color>");
        Debug.Log("────────────────────────────────────────");
    }
}