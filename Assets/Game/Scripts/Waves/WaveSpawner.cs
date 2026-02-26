using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Glow Effect")]
    [SerializeField] private Color glowColor = Color.blue;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float glowDuration = 0.2f;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Called before first wave only — builds the initial wave queue during countdown.
    /// </summary>
    public void PrepareQueue(WaveData wave)
    {
        Debug.Log($"[WaveSpawner] 🔥 PrepareQueue() for: <color=white>{wave.waveName}</color> | Enemies: {wave.enemies.Length}");
        EnemyPool.Instance.BuildWaveQueue(wave.enemies);
        EnemyPool.Instance.LogPoolState();
    }

    public IEnumerator SpawnWave(WaveData wave, System.Action onEnemySpawned)
    {
        Debug.Log($"[WaveSpawner] 🚀 SpawnWave() | <color=white>{wave.waveName}</color> | Enemies: <color=yellow>{wave.enemies.Length}</color>");

        int index = 0;
        while (EnemyPool.Instance.QueueCount > 0)
        {
            index++;
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            MeshRenderer meshRend = spawnPoint.GetComponent<MeshRenderer>();

            Debug.Log($"[WaveSpawner] 👾 Spawning {index}/{wave.enemies.Length} at {spawnPoint.name}");

            if (meshRend != null)
            {
                meshRend.enabled = true;
                yield return StartCoroutine(GlowEffect(meshRend));
                yield return new WaitForSeconds(0.2f);
            }

            EnemyPool.Instance.GetNext(spawnPoint.position, Quaternion.identity);
            onEnemySpawned?.Invoke();

            if (meshRend != null)
            {
                yield return new WaitForSeconds(0.3f);
                meshRend.enabled = false;
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        Debug.Log($"[WaveSpawner] 🏁 All enemies spawned for: <color=white>{wave.waveName}</color>");
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