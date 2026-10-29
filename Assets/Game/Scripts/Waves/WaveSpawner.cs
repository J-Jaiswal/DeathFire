using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Glow Effect")]
    [SerializeField] private Color glowColor = Color.blue;
     private readonly float glowIntensity = 10f;
    [SerializeField] private float pulseSpeed = 2f;
     private readonly float glowDuration = 1.8f;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    public void PrepareQueue(WaveData wave)
    {
        //Debug.Log($"[WaveSpawner] PrepareQueue: {wave.waveName} | Enemies: {wave.enemies.Length}");
        EnemyPool.Instance.BuildWaveQueue(wave.enemies);
        EnemyPool.Instance.LogPoolState();
    }

    // Callback now passes back the spawned GameObject
    public IEnumerator SpawnWave(WaveData wave, System.Action<GameObject> onEnemySpawned)
    {
        //Debug.Log($"[WaveSpawner] SpawnWave: {wave.waveName} | Enemies: {wave.enemies.Length}");

        int index = 0;
        while (EnemyPool.Instance.QueueCount > 0)
        {
            index++;
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            MeshRenderer meshRend = spawnPoint.GetComponent<MeshRenderer>();

            //Debug.Log($"[WaveSpawner] Spawning {index}/{wave.enemies.Length} at {spawnPoint.name}");

            if (meshRend != null)
            {
                meshRend.enabled = true;
               
                yield return StartCoroutine(GlowEffect(meshRend));
                yield return new WaitForSeconds(0.3f);
            }

            // Capture the spawned enemy
            GameObject spawnedEnemy = EnemyPool.Instance.GetNext(spawnPoint.position, Quaternion.identity);

            // Register IMMEDIATELY so any death during the post-spawn glow delay is counted.
            // Previously this was invoked after a 1-second wait, causing a race:
            // if the enemy died in that window, NotifyEnemyDeath was ignored and
            // enemiesAlive never reached 0, so the next-wave panel never appeared.
            onEnemySpawned?.Invoke(spawnedEnemy);

            if (meshRend != null)
            {
                yield return new WaitForSeconds(1f);
                meshRend.enabled = false;
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        //Debug.Log($"[WaveSpawner] All enemies spawned: {wave.waveName}");
    }

    private IEnumerator GlowEffect(MeshRenderer rend)
    {
        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            float pulse = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(elapsed * pulseSpeed, 1f));
            mpb.SetColor("_EmissionColor", glowColor * pulse * glowIntensity);
            rend.SetPropertyBlock(mpb);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mpb.SetColor("_EmissionColor", Color.black);
        rend.SetPropertyBlock(mpb);
    }
}