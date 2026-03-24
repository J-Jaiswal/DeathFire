using UnityEngine;
public enum MovementType
{
    Straight,
    ZigZag,
    Circle,
    Static, 
    Elite
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("General Setting")]

    [SerializeField] public  string enemyName;  
    [SerializeField] public MovementType movementType;
    [SerializeField] public int ScoreValue ;

    [Header("Health Setting")]

    [SerializeField] public int maxHealth;   
    [SerializeField] public int damageToPlayer;

    [Header("Speed Setting")]
    [SerializeField] public float accelerate = 0.05f;
    [SerializeField] public float moveSpeed;


}
