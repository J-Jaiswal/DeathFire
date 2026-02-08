using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Toolbars;

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
           SpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }   
    }

   public void SpawnWave()
    {
         Transform RandomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
         GameObject RandomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];


        MeshRenderer meshRend = RandomSpawnPoint.GetComponent<MeshRenderer>();
        meshRend.enabled = true;
      

        StartCoroutine(GLowEffect(meshRend));   

        meshRend.enabled = false;

        Instantiate(RandomEnemyPrefab, RandomSpawnPoint.position, Quaternion.identity);

        
    }   

    private IEnumerator GLowEffect(MeshRenderer rend)
    {
       
        if (rend == null) { 
            Debug.LogWarning("Spawn point does not have a Renderer component for glow effect.");
           yield break ;
        }
        rend.enabled = true;
        float elapsedTime = 0f;

        while ( elapsedTime < glowDuration)
        {
            float pulse = Mathf.PingPong(elapsedTime * pulseSpeed, 1f);
            pulse = Mathf.SmoothStep(0f, 1f, pulse);

            Color emission = glowColor * pulse * glowIntensity; 

            mpb.SetColor("_EmissionColor", emission);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
       
        
        //mpb.SetColor("_EmissionColor", Color.black);
        rend.SetPropertyBlock(mpb);
     
  
    }

}
