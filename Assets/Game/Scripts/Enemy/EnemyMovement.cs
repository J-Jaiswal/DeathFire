using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float circleFrequency = 2f;
    [SerializeField] private float circleRadius = 1f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float contactDistance = 1f;

    private MovementType movementType;
    private float moveSpeed;
    private bool hasDealtDamage; // prevents multi-hit before pool return

    public void Initialize(MovementType type, float speed)
    {
        movementType = type;
        moveSpeed = speed;
        hasDealtDamage = false; // reset on reuse from pool
    }

    private void OnEnable()
    {
        hasDealtDamage = false;

        // Auto-find player if not assigned in Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning($"[EnemyMovement] No Player found for {gameObject.name}");
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
    }

    private void CheckContactDamage()
    {
        if (Vector3.Distance(transform.position, player.position) > contactDistance) return;

        hasDealtDamage = true;

        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.TakeDamage(damageAmount);
        else
            Debug.LogWarning("[EnemyMovement] PlayerHealth.Instance is null!");

        Debug.Log($"[Enemy] {gameObject.name} dealt {damageAmount} damage, returning to pool.");
        EnemyPool.Instance.Return(gameObject);
    }
}