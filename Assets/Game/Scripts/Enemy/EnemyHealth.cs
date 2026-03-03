using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    private int damageAmount;
    private int killScore;  
    [SerializeField] private Slider healthBar;
    [SerializeField] Transform target;

    [SerializeField] ParticleSystem hitVfx;
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject deathFVX;
    public Gradient healthGradient;

    [Header("References")]
    //private ScoreManager scoreManager;
    //private WaveManager waveManager;  // FIX 4: Cache instead of FindAnyObjectByType every death

    [Header("Settings")]
    [SerializeField]
    private bool canBeHeadshot = true;

    private void Awake()
    {
        //scoreManager = FindAnyObjectByType<ScoreManager>();
        //waveManager = FindAnyObjectByType<WaveManager>();  // FIX 4: Cache on Awake
    }

    public void Initialize(int health, int damage ,  int killScore)
    {
        maxHealth = health;
        damageAmount = damage;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthBarImage.color = healthGradient.Evaluate(1f);
        currentHealth = maxHealth;
        this.killScore = killScore; 
    }

    void HealthBarFace()
    {
        if (target != null)
        {
            Vector3 direction = target.position - healthBar.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            healthBar.transform.rotation = lookRotation;
        }
    }

    void Update()
    {
        HealthBarFace();
    }

    public void TakeDamage(int damage, Vector3 hitpoint, bool headshot = false)
    {
        bool actualHeadshot = headshot && canBeHeadshot;
        currentHealth -= damage;
        healthBar.value = currentHealth;
        healthBarImage.color = healthGradient.Evaluate((float)currentHealth / maxHealth);
        if (hitVfx != null)
        {
            hitVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitVfx.Play();
        }
        if (currentHealth <= 0)
        {
            Die(actualHeadshot);
        }
    }

    void Die(bool isHeadshot)
    {
        if (deathFVX != null)
        {
            // FIX 3: Capture the clone returned by Instantiate and destroy THAT,
            // not deathFVX itself (which is the serialized prefab reference).
            GameObject clone = Instantiate(deathFVX, transform.position, Quaternion.identity);
            Destroy(clone, 2f);
        }

        ScoreManager.Instance.ReportKill(isHeadshot , killScore );
        
        //WaveStatsTracker.Instance?.RegisterKill(isHeadshot);
        WaveSpawnController.Instance?.NotifyEnemyDeath(gameObject);
        Debug.Log($"[Enemy] 🔄 {gameObject.name} calling EnemyPool.Return()...");
        EnemyPool.Instance.Return(gameObject);
        //Destroy(gameObject);
    }
}