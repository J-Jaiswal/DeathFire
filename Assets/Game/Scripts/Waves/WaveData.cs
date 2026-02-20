using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]


public class WaveData : ScriptableObject
{

    public string waveName;
    public GameObject[] enemies;  
    public float spawnInterval = 1f;
    public bool fullAmmoRefil;
    public int ammoRefilAmount; 
    public bool healthDrop;
    public float healthDropChance;  

}


