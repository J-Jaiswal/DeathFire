using UnityEngine;
using System.Collections;


public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]  
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;    

    private float spawnInterval = 3f;

    [Header("Glow Effect")]
                     
    [SerializeField] private Color glowColor = Color.blue;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float pulseSpeed = 2f; 
    [SerializeField] private float glowDuration = 0.5f;
    private MaterialPropertyBlock mpb;
    private void Start()
    {
        mpb = new MaterialPropertyBlock();

        StartCoroutine(SpawnLoop(spawnInterval));

    }

    private IEnumerator SpawnLoop(  float spawnInterval)
    {
         while (true)
        {
          StartCoroutine(SpawnRoutine());   
            yield return new WaitForSeconds(spawnInterval);    
        }   
    }


    private IEnumerator SpawnRoutine()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        MeshRenderer meshRend = spawnPoint.GetComponent<MeshRenderer>();
        if (meshRend == null)
            yield break;

     
        meshRend.enabled = true;

       
        yield return StartCoroutine(GlowEffect(meshRend));

        
        yield return new WaitForSeconds(0.2f);

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);

        meshRend.enabled = false;
    }


    private IEnumerator GlowEffect(MeshRenderer rend)
    {
        float elapsedTime = 0f;

        while (elapsedTime < glowDuration)
        {
            float pulse = Mathf.PingPong(elapsedTime * pulseSpeed, 1f);
            pulse = Mathf.SmoothStep(0f, 1f, pulse);

            Color emission = glowColor * pulse * glowIntensity;

            mpb.SetColor("_EmissionColor", emission);
            rend.SetPropertyBlock(mpb);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


}
