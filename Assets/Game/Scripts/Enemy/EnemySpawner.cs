using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;    

    private float spawnInterval = 3f;  
    private int enemyCount;
    private int spawnerNum;
    Material spawnGlow;

    private void Start()
    {
     
        //spawnGlow = GetComponent<Renderer>().material;  

        enemyCount = enemyPrefabs.Length;

        StartCoroutine(SpawnLoop(spawnInterval));   


    }


    private IEnumerator SpawnLoop(  float spawnInterval)
    {
         while (true)
        {
           SpawnWave(enemyCount, enemyPrefabs);
            yield return new WaitForSeconds(spawnInterval);
        }   
    }

   public void SpawnWave( int enemyCount, GameObject[] enemyPrefabs)
    {
        //enemyCount = Mathf.Max(1, enemyCount); // Ensure at least one enemy is spawned  
        for (int i = 0; i < enemyCount; i++)
        {
            Transform RandomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject RandomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(RandomEnemyPrefab, RandomSpawnPoint.position, Quaternion.identity);
            
        }
    }   

}
