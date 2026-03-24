using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{

    public string waveName;
    public string enemyDetails;
    public GameObject[] enemies;  
    public float spawnInterval = 1f;
    public bool fullAmmoRefill;
    public int ammoRefillAmount; 
    //public bool healthDrop;
    //public float healthDropChance;  

}


