using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private RawImage healthBarFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private GameObject deathPanel;
    [Header("Death Image Pulse on Hit")]
    [SerializeField] private Image deathImage;
    [SerializeField] private float hitPulseMaxAlpha = 0.5f;
    [SerializeField] private float hitPulseDuration = 1f;

private Coroutine hitPulseCoroutine;

    public static PlayerHealth Instance { get; private set; }

    //public bool IsDead => isDead;
    //public int CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private int currentHealth;
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

        if (deathImage != null)
            deathImage.color = new Color(1, 0, 0, 0);

        RefreshUI();
    }

    // ── Public API ────────────────────────────────────────────────────────────

   

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(  maxHealth, currentHealth + amount);
        RefreshUI();
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        float normalized = (float)currentHealth / maxHealth;
        if (healthBar != null)
            healthBar.value = normalized;
        if (healthBarFill != null)
            healthBarFill.color = healthGradient.Evaluate(normalized);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        RefreshUI();
        //TriggerDamageFlash();
        TriggerDeathImagePulse();
        SFXManager.Instance.PlayPlayerHurt();
        if (currentHealth <= 0) { isDead = true; OnDeath(); }
    }

    private void TriggerDeathImagePulse()
    {
        if (deathImage == null) return;
        if (hitPulseCoroutine != null) StopCoroutine(hitPulseCoroutine);
        hitPulseCoroutine = StartCoroutine(HitPulseRoutine());
    }

    private IEnumerator HitPulseRoutine()
    {
        Color c = deathImage.color;
        float half = hitPulseDuration / 2f;

        // Fade in: 0 → max
        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            deathImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, hitPulseMaxAlpha, elapsed / half));
            yield return null;
        }

        // Fade out: max → 0
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            deathImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(hitPulseMaxAlpha, 0f, elapsed / half));
            yield return null;
        }

        deathImage.color = new Color(c.r, c.g, c.b, 0f);
    }

    private void OnDeath()
    {

        int waveReached = WaveManager.Instance != null ? WaveManager.Instance.CurrentWaveNumber : 0;
        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.TotalScore : 0;

        WaveStatsTracker.Instance.CommitRunResults(finalScore, waveReached);
        SFXManager.Instance.PlayPlayerDeath();
        if (deathPanel != null) deathPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Player died!");
    }
}