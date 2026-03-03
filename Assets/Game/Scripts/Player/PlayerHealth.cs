using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private GameObject deathPanel;


    [Header("Hit Flash")]
    [SerializeField] private Image damageFlashOverlay;
    [SerializeField] private float flashDuration = 0.15f;

    public static PlayerHealth Instance { get; private set; }

    public bool IsDead => isDead;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private float currentHealth;
    private bool isDead;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        // Singleton setup belongs in Awake, not Start
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PlayerHealth instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (damageFlashOverlay != null)
            damageFlashOverlay.color = new Color(1, 0, 0, 0);

        RefreshUI();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        RefreshUI();
        TriggerDamageFlash();

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnDeath();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        RefreshUI();
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        float normalized = currentHealth / maxHealth;

        if (healthBar != null)
            healthBar.value = normalized;

        if (healthBarFill != null)
            healthBarFill.color = healthGradient.Evaluate(normalized);
    }

    private void TriggerDamageFlash()
    {
        if (damageFlashOverlay == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Instant show
        damageFlashOverlay.color = new Color(1, 0, 0, 0.35f);

        // Smooth fade out over flashDuration
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.35f, 0f, elapsed / flashDuration);
            damageFlashOverlay.color = new Color(1, 0, 0, alpha);
            yield return null;
        }

        damageFlashOverlay.color = new Color(1, 0, 0, 0f);
    }

    private void OnDeath()
    {
        deathPanel.SetActive(true);  
        Time.timeScale = 0f; 
        Debug.Log("Player died!");
        
    }
}