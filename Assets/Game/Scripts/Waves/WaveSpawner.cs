using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

  
    public IEnumerator SpawnWave(WaveData wave, System.Action onEnemySpawned)
    {
        foreach (GameObject enemyPrefab in wave.enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            MeshRenderer meshRend = spawnPoint.GetComponent<MeshRenderer>();

            if (meshRend != null)
            {
                meshRend.enabled = true;
                yield return StartCoroutine(GlowEffect(meshRend));
                yield return new WaitForSeconds(0.2f);
            }

            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            onEnemySpawned?.Invoke();

            if (meshRend != null)
            {
                yield return new WaitForSeconds(0.3f);
                meshRend.enabled = false;
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private IEnumerator GlowEffect(MeshRenderer rend)
    {
        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            float pulse = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(elapsed * pulseSpeed, 1f));
            Color emission = glowColor * pulse * glowIntensity;
            mpb.SetColor("_EmissionColor", emission);
            rend.SetPropertyBlock(mpb);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset emission when done
        mpb.SetColor("_EmissionColor", Color.black);
        rend.SetPropertyBlock(mpb);
    }
}