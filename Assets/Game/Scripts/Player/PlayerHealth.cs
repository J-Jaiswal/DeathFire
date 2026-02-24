using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;  // the colored fill image
    [SerializeField] private Gradient healthGradient; // set green→yellow→red in Inspector

    [Header("Hit Flash")]
    [SerializeField] private Image damageFlashOverlay; // full-screen red Image, low alpha
    [SerializeField] private float flashDuration = 0.15f;

    private float currentHealth;
    private bool isDead;
    private Coroutine flashCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        if (damageFlashOverlay != null)
            damageFlashOverlay.color = new Color(1, 0, 0, 0); // start invisible
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

        if (healthBar != null) healthBar.value = normalized;
        if (healthText != null) healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";
        if (healthBarFill != null) healthBarFill.color = healthGradient.Evaluate(normalized);
    }

    private void TriggerDamageFlash()
    {
        if (damageFlashOverlay == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        damageFlashOverlay.color = new Color(1, 0, 0, 0.35f); // show red flash
        yield return new WaitForSeconds(flashDuration);
        damageFlashOverlay.color = new Color(1, 0, 0, 0f);    // fade out
    }

    private void OnDeath()
    {
        // stub — hook up game over screen here later
        Debug.Log("Player died!");
    }
}
