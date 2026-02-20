using UnityEngine;
public enum MovementType
{
    Straight,
    ZigZag,
    Circle,
    Static
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] public  string enemyName;  
    [SerializeField] public MovementType movementType;   
    [SerializeField] public int maxHealth;   
    [SerializeField] public float moveSpeed;
    [SerializeField] public int ScoreValue ;
    [SerializeField] public int damageToPlayer; 




}
