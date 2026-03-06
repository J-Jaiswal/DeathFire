using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    private int killScore;  
    [SerializeField] private Slider healthBar;
  

    [SerializeField] ParticleSystem hitVFX;
    [SerializeField] ParticleSystem headVFX;
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject deathFVX;
    public Gradient healthGradient;

   
    public void Initialize(int health,  int killScore)
    {
        maxHealth = health;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthBarImage.color = healthGradient.Evaluate(1f);
        currentHealth = maxHealth;
        this.killScore = killScore; 
    }

   

    public void TakeDamage(int damage, bool headshot)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;
        healthBarImage.color = healthGradient.Evaluate((float)currentHealth / maxHealth);

        if (headshot)
        {
            HeadEffect();
            ScoreManager.Instance.ReportHeadshot(); 
        }
        else
        {
            HitEffect();
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (deathFVX != null)
        {
            GameObject clone = Instantiate(deathFVX, transform.position, Quaternion.identity);
            Destroy(clone, 2f);
        }

        ScoreManager.Instance.ReportKill(killScore); 
        WaveSpawnController.Instance.NotifyEnemyDeath(gameObject);
        EnemyPool.Instance.Return(gameObject);
    }

    private void HitEffect()
    {
        if (hitVFX != null)
        {
            hitVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitVFX.Play();
        }
    }

    private void HeadEffect()
    {
        if ( headVFX != null)
        {
            headVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            headVFX.Play();
        }
    }
}