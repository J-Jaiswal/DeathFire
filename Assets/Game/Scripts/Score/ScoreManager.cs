using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Values")]
    [SerializeField] private int waveCompletedBonus = 500;
    [SerializeField] private int passiveScorePerTick = 10;
    [SerializeField] private float passiveScoreInterval = 2f;

    [Header("Combo Settings")]
    [SerializeField] private float comboWindow = 2f;
    [SerializeField] private int comboThreshold = 2;
    [SerializeField] private float comboMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Floating Text")]
    [SerializeField] private FloatingText killFloatingText;
    [SerializeField] private FloatingText headshotFloatingText;
    [SerializeField] private FloatingText comboFloatingText;

    public int TotalScore  { get; private set; }
    private int recentKillCount = 0;
    private float lastKillTime = -999f;
    private float passiveTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        TotalScore = 0;
        scoreText.text = "00"; 
    }

    private void Update()
    {

        if (!WaveSpawnController.Instance.WaveInProgress) return;
        passiveTimer += Time.deltaTime;
        if (passiveTimer >= passiveScoreInterval)
        {
            passiveTimer = 0f;
            AddScore(passiveScorePerTick);
        }

        if (recentKillCount > 0 && Time.time - lastKillTime > comboWindow)
            recentKillCount = 0;
    }

    // Called on every headshot hit — visual feedback only, no score
    public void ReportHeadshot()
    {
        headshotFloatingText.Show("HEADSHOT!", Color.red);
    }

    // Called only on kill — score, combo, kill text
    public void ReportKill(int killScore)
    {
        int earned = killScore;

        float timeSinceLast = Time.time - lastKillTime;
        recentKillCount = timeSinceLast <= comboWindow ? recentKillCount + 1 : 1;
        lastKillTime = Time.time;

        if (recentKillCount >= comboThreshold)
        {
            int comboBonus = Mathf.RoundToInt(earned * comboMultiplier);
            ShowComboText(recentKillCount, comboBonus);
            earned += comboBonus;
        }

        killFloatingText.Show($"+{earned}", Color.white);
        AddScore(earned);
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
        comboFloatingText.Show($"{label}", Color.cyan);
    }

    public void ReportWaveCompleted()
    {
        AddScore(waveCompletedBonus);
        comboFloatingText.Show($"+{waveCompletedBonus}  WAVE CLEAR!", Color.yellow);
    }

    private void AddScore(int amount)
    {
        TotalScore += amount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = TotalScore.ToString("N0");
    }
}