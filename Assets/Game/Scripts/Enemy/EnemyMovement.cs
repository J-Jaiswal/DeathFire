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
    private MovementType movementType;
    //[SerializeField] GameObject deathFVX;

    // Speed scaling
     [SerializeField]  private float moveSpeed;
    private float baseSpeed;
    private float speedScaleRate;
    private float elapsedTime;
    // Elite movement
    [SerializeField] private float elitePatternChangInterval = 2.5f;
    private float elitePatternTimer;
    private MovementType eliteCurrentPattern;
    private readonly MovementType[] elitePatternPool = new[]
    {
    MovementType.ZigZag,
    MovementType.Circle,
    MovementType.Straight
};


    private bool hasDealtDamage; // prevents multi-hit before pool return

    public void Initialize(MovementType type, float speed, int damageToPlayer, float accelerate)
    {
        movementType = type;
        moveSpeed = speed;
        baseSpeed = speed;
        damageAmount = damageToPlayer;
        speedScaleRate = accelerate;
        hasDealtDamage = false;
        elapsedTime = 0f;

        if (movementType == MovementType.Elite)
            PickNewElitePattern();
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
    private void PickNewElitePattern()
    {
        // Pick any pattern except the one currently active
        MovementType next;
        do
        {
            next = elitePatternPool[Random.Range(0, elitePatternPool.Length)];
        }
        while (next == eliteCurrentPattern && elitePatternPool.Length > 1);

        eliteCurrentPattern = next;
        elitePatternTimer = elitePatternChangInterval;

        Debug.Log($"[Elite] {gameObject.name} switched to {eliteCurrentPattern}");
    }
    private void Update()
    {
        if (player == null || hasDealtDamage) return;

        elapsedTime += Time.deltaTime;

        // --- Speed scaling over time ---
       
        moveSpeed = baseSpeed + elapsedTime * speedScaleRate;
        // --- Elite pattern switching ---
        if (movementType == MovementType.Elite)
        {
            elitePatternTimer -= Time.deltaTime;
            if (elitePatternTimer <= 0f)
                PickNewElitePattern();
        }

        // --- Movement ---
        MovementType activePattern = movementType == MovementType.Elite
            ? eliteCurrentPattern
            : movementType;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 offset = Vector3.zero;

        switch (activePattern)
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

        if (activePattern != MovementType.Static)
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
           
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation * Quaternion.Euler(0f, 180f, 0f); // flip if model faces wrong way
        }
    }
    private void CheckContactDamage()
    {
        if (Vector3.Distance(transform.position, player.position) > contactDistance) return;
        hasDealtDamage = true;
       
        PlayerHealth.Instance.TakeDamage(damageAmount);
        WaveSpawnController.Instance.NotifyEnemyDeath(gameObject);

        EnemyPool.Instance.Return(gameObject);

        Debug.Log($"[Enemy] {gameObject.name} dealt {damageAmount} damage, returning to pool.");
    }


   
}