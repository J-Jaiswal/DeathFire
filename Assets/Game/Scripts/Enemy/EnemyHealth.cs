using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int damageAmount = 40;
    [SerializeField] Slider healthBar;
    [SerializeField] Transform target;

    [SerializeField] ParticleSystem hitVfx;
    [SerializeField] Image healthBarImage;
    public Gradient healthGradient;
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();   
        healthBarImage = healthBar.GetComponentInChildren<Image>();
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
        healthBarImage.color = healthGradient.Evaluate((float)currentHealth / 100); 
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
        Destroy(gameObject);
    }   
}
