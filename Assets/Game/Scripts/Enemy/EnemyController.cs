using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("References")]
    [SerializeField] private Transform player;

    private EnemyMovement movement;
    private EnemyHealth health;

    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
    }

    void Start()
    {
        // adding raw data to component by dependency injection  
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        // Initialize Movement
        movement.Initialize(
            enemyData.movementType,
            enemyData.moveSpeed
        );

        // Initialize Health
        health.Initialize(
            enemyData.maxHealth,
            enemyData.damageToPlayer
        );
    }
}