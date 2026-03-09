using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float circleFrequency = 2f;
    [SerializeField] private float circleRadius = 1f;
    private int damageAmount ;
    private float contactDistance = 3f;
    //[SerializeField] GameObject deathFVX;


    private MovementType movementType;
    private float moveSpeed;
    private bool hasDealtDamage; // prevents multi-hit before pool return

    public void Initialize(MovementType type, float speed , int damageToPlayer)
    {
        movementType = type;
        moveSpeed = speed;
        damageAmount = damageToPlayer;
        hasDealtDamage = false; // reset on reuse from pool
    }

    private void OnEnable()
    {
        hasDealtDamage = false;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("EnemyTarget");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning($"[EnemyMovement] EnemyTarget found for {gameObject.name}");
        }
    }

    private void Update()
    {
        if (player == null || hasDealtDamage) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 offset = Vector3.zero;

        switch (movementType)
        {
            case MovementType.ZigZag:
                offset = transform.right * Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
                break;

            case MovementType.Circle:
                offset = new Vector3(
                    Mathf.Cos(Time.time * circleFrequency),
                    Mathf.Sin(Time.time * circleFrequency),
                    0f
                ) * circleRadius;
                break;
        }

        if (movementType != MovementType.Static)
        {
            Vector3 finalDir = (toPlayer + offset).normalized;
            transform.position += moveSpeed * finalDir * Time.deltaTime;
        }

        CheckContactDamage();
        EnemyFace();
    }

    private void EnemyFace()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            //direction.y = 0f;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation * Quaternion.Euler(0f, 180f, 0f); // flip if model faces wrong way
        }
    }
    private void CheckContactDamage()
    {
        if (Vector3.Distance(transform.position, player.position) > contactDistance) return;
        hasDealtDamage = true;
        //if (deathFVX != null)
        //{
        //    GameObject clone = Instantiate(deathFVX, transform.position, Quaternion.identity);
        //    Destroy(clone, 2f);
        //}

        PlayerHealth.Instance.TakeDamage(damageAmount);
        WaveSpawnController.Instance?.NotifyEnemyDeath(gameObject);

        EnemyPool.Instance.Return(gameObject);

        Debug.Log($"[Enemy] {gameObject.name} dealt {damageAmount} damage, returning to pool.");
    }


   
}