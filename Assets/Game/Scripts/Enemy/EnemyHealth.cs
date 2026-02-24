using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{

    private int maxHealth;
    private int currentHealth;
    private int damageAmount;
    [SerializeField] private Slider healthBar;
    [SerializeField] Transform target;

    [SerializeField] ParticleSystem hitVfx;
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject deathFVX;
    public Gradient healthGradient;

    //[SerializeField] private FloatingText floatingText;
    //[SerializeField] private int scoreValue = 100;



    public void Initialize(int health, int damage)
    {
        maxHealth = health;
        damageAmount = damage;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthBarImage.color = healthGradient.Evaluate(1f);
        currentHealth = maxHealth;
      
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



    public void TakeDamage(int damage , Vector3 hitpoint)
    {
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
            Die();
        }
    }  
    
    void Die()
    {
        if ( deathFVX != null)
        {
           Instantiate(deathFVX, transform.position, Quaternion.identity);
           Destroy( deathFVX, 2f);
        }

        FindAnyObjectByType<WaveManager>().NotifyEnemyDeath(); 
        //floatingText.Show($"+{scoreValue}", Color.white);
        Destroy(gameObject);
    }  
    
  
}
