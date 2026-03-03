using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Values")]
    //[SerializeField] private int killScore = 100;
    [SerializeField] private int headshotBonus = 60;
    [SerializeField] private int waveCompletedBonus = 500;
    [SerializeField] private int passiveScorePerTick = 10;
    [SerializeField] private float passiveScoreInterval = 2f;

    [Header("Combo Settings")]
    [SerializeField] private float comboWindow = 2f;       // seconds to land kills in
    [SerializeField] private int comboThreshold = 2;       // kills needed to trigger combo
    [SerializeField] private float comboMultiplier = 1.5f; // bonus multiplier on top of base

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("References")]
    [SerializeField] private FloatingText killFloatingText;
    [SerializeField] private FloatingText comboFloatingText;

    private int totalScore = 0;

    // Combo tracking
    private int recentKillCount = 0;
    private float lastKillTime = -999f;

    // Passive score
    private float passiveTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        passiveTimer += Time.deltaTime;
        if (passiveTimer >= passiveScoreInterval)
        {
            passiveTimer = 0f;
            AddScore(passiveScorePerTick);
            // No floating text for passive ticks — keeps screen clean
        }

        // Reset combo counter if window has passed without a new kill
        if (recentKillCount > 0 && Time.time - lastKillTime > comboWindow)
            recentKillCount = 0;
    }

    public void ReportKill(bool isHeadshot , int killScore)
    {
        int earned = killScore;

        // --- Combo check ---
        float timeSinceLast = Time.time - lastKillTime;
        if (timeSinceLast <= comboWindow)
            recentKillCount++;
        else
            recentKillCount = 1;

        lastKillTime = Time.time;

        bool isCombo = recentKillCount >= comboThreshold;
        int comboBonus = 0;

        if (isCombo)
        {
            comboBonus = Mathf.RoundToInt(earned * comboMultiplier);
            ShowComboText(recentKillCount, comboBonus);
        }

        // --- Headshot label (kill floating text) ---
        if (isHeadshot)
        {
            earned += headshotBonus;
            killFloatingText?.Prepare($"HEADSHOT! +{earned}", Color.red);
            if (killFloatingText != null) killFloatingText.ShowText = true;
        }
        else
        {
            killFloatingText?.Prepare($"+{earned}", Color.white);
            if (killFloatingText != null) killFloatingText.ShowText = true;
        }

        AddScore(earned + comboBonus);
    }

    private void ShowComboText(int killsInWindow, int bonus)
    {
        string label = killsInWindow switch
        {
            2 => "DOUBLE KILL!",
            3 => "TRIPLE KILL!",
            4 => "QUAD KILL!",
            _ => $"{killsInWindow}x KILL!"
        };

        comboFloatingText?.Prepare($"{label}  +{bonus}", Color.cyan);
        if (comboFloatingText != null) comboFloatingText.ShowText = true;
    }

    public void ReportWaveCompleted()
    {
        AddScore(waveCompletedBonus);
        comboFloatingText?.Prepare($"+{waveCompletedBonus}  WAVE CLEAR!", Color.yellow);
        if (comboFloatingText != null) comboFloatingText.ShowText = true;
    }

    private void AddScore(int amount)
    {
        totalScore += amount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = totalScore.ToString("N0");
    }
}