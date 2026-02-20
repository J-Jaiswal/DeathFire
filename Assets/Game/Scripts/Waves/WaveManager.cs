using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;


public class WaveManager : MonoBehaviour
{

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] Transform[] spawnPoints;
    private int currentWaveIndex = 0;  
    private WaveData currentWave;       
    private float timeDelayNextWave = 5f;   

    [Header("Glow Effect")]

    [SerializeField] private Color glowColor = Color.blue;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float glowDuration = 0.2f;
    private MaterialPropertyBlock mpb;
    bool waveCompleted = false; 
    public int enemyAlive =  0;
    private bool isSpawning = false;    
    private bool waveInProgreess = false;

    [Header("Debug")]

    [SerializeField] private TextMeshProUGUI waveUI;


    private void Start()
    {
        mpb = new MaterialPropertyBlock();
        StartCoroutine(SpawnNextWave());
    }


    IEnumerator SpawnNextWave()
    {
       if ( currentWaveIndex >= waves.Count)
        {
            Debug.Log("No more waves to spawn!");
            yield break;
        }
       yield return new WaitForSeconds(  timeDelayNextWave );
        
       currentWave = waves[currentWaveIndex];   
        waveUI.text = $"Wave {currentWaveIndex + 1}";
        waveInProgreess = true;
       yield return StartCoroutine(SpawnWave());   

        isSpawning = false;

        

       checkWaveCompletion();   
       
    }

    IEnumerator SpawnWave()
    {
        enemyAlive = 0;
        foreach (GameObject enemyPrefab in currentWave.enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // spawn gate glow effect --->  
            MeshRenderer meshRend = spawnPoint.GetComponent<MeshRenderer>();
            meshRend.enabled = true;
            yield return StartCoroutine(GlowEffect(meshRend));
            yield return new WaitForSeconds(0.2f);
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            meshRend.enabled = false;
            // spawn gate glow effect --->  

            enemyAlive++;
            Debug.Log("Enemies Alive: " + enemyAlive);  
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }
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

    public void NotifyEnemyDeath()
    {
        enemyAlive--;
        Debug.Log("Enemy defeated! Remaining: " + enemyAlive);  

        checkWaveCompletion();
     
    }

    void checkWaveCompletion()
    {
       if ( enemyAlive <= 0 && !isSpawning  && waveInProgreess)
        {
            Debug.Log("Wave completed!");
            waveInProgreess = false;
           
            currentWaveIndex++; 

            StartCoroutine(SpawnNextWave());        
        }       
    }
    bool WaveCompleted ()
    {
        waveCompleted = true;
        return waveCompleted ;
    }   


}
