using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveUIManager : MonoBehaviour
{
    // ── HUD (always visible during wave) ────────────────────────
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI primaryText;
    [SerializeField] private TextMeshProUGUI secondaryText;

    [Header("Wave Number Display")]
    [SerializeField] private TextMeshProUGUI waveNumberText;

    // ── Between-Wave Panel ───────────────────────────────────────
    [Header("Between-Wave Panel")]
    [SerializeField] private GameObject betweenWavePanel;

    [Header("  Left — Wave Stats")]
    [SerializeField] private TextMeshProUGUI statsHeaderText;
    [SerializeField] private TextMeshProUGUI statsKillsText;
    [SerializeField] private TextMeshProUGUI statsHeadshotsText;
    [SerializeField] private TextMeshProUGUI statsAccuracyText;

    [Header("  Right — Next Wave Info")]
    [SerializeField] private TextMeshProUGUI nextWaveTitleText;
    [SerializeField] private TextMeshProUGUI nextWaveEnemyListText;
    [SerializeField] private TextMeshProUGUI nextWaveDropsText;

    [Header("  Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseButtonLabel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // ── Internal ─────────────────────────────────────────────────
    private Coroutine primaryCoroutine;
    private Coroutine secondaryCoroutine;
    private bool isPaused = false;

    public bool IsPanelOpen { get; private set; }

    // ── Unity Lifecycle ──────────────────────────────────────────

    private void Start()
    {
        // HUD refs
        if (primaryText == null) Debug.LogWarning("[WaveUIManager] primaryText not assigned!");
        if (secondaryText == null) Debug.LogWarning("[WaveUIManager] secondaryText not assigned!");

        // Panel refs
        if (betweenWavePanel == null) Debug.LogWarning("[WaveUIManager] betweenWavePanel not assigned!");
        if (statsKillsText == null) Debug.LogWarning("[WaveUIManager] statsKillsText not assigned!");
        if (statsHeadshotsText == null) Debug.LogWarning("[WaveUIManager] statsHeadshotsText not assigned!");
        if (statsAccuracyText == null) Debug.LogWarning("[WaveUIManager] statsAccuracyText not assigned!");
        if (nextWaveTitleText == null) Debug.LogWarning("[WaveUIManager] nextWaveTitleText not assigned!");
        if (nextWaveEnemyListText == null) Debug.LogWarning("[WaveUIManager] nextWaveEnemyListText not assigned!");
        if (nextWaveDropsText == null) Debug.LogWarning("[WaveUIManager] nextWaveDropsText not assigned!");
        if (pauseButton == null) Debug.LogWarning("[WaveUIManager] pauseButton not assigned!");
        if (pauseButtonLabel == null) Debug.LogWarning("[WaveUIManager] pauseButtonLabel not assigned!");
        if (mainMenuButton == null) Debug.LogWarning("[WaveUIManager] mainMenuButton not assigned!");
        if (quitButton == null) Debug.LogWarning("[WaveUIManager] quitButton not assigned!");

        betweenWavePanel?.SetActive(false);
        ClearAll();

        pauseButton?.onClick.AddListener(OnPauseClicked);
        mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
        quitButton?.onClick.AddListener(OnQuitClicked);
    }

    // ── Between-Wave Panel ───────────────────────────────────────

    /// <summary>
    /// Opens panel, populates both sides, runs countdown inside.
    /// WaveManager yields on this — it returns when countdown finishes.
    /// </summary>
    /// 
    public void SetWaveNumber(int waveNumber)
    {
        if (waveNumberText == null) return;
        waveNumberText.text = $"Wave {waveNumber}";
        waveNumberText.gameObject.SetActive(true);
    }

    public void HideWaveNumber()
    {
        if (waveNumberText == null) return;
        waveNumberText.gameObject.SetActive(false);
    }
    public IEnumerator ShowBetweenWavePanel(int completedWaveNumber, float countdownDuration)
    {
        IsPanelOpen = true;

        PopulateStats(completedWaveNumber);
        PopulateNextWaveInfo();

        betweenWavePanel?.SetActive(true);

        yield return StartCoroutine(BetweenWaveCountdown(countdownDuration, completedWaveNumber + 1));

        CloseBetweenWavePanel();
    }

    private void CloseBetweenWavePanel()
    {
        betweenWavePanel?.SetActive(false);
        IsPanelOpen = false;
        if (isPaused) ResumeGame();
    }

    // ── Populate Left: Stats ─────────────────────────────────────

    private void PopulateStats(int completedWaveNumber)
    {
        if (statsHeaderText != null)
            statsHeaderText.text = $"WAVE {completedWaveNumber} STATS";

        var t = WaveStatsTracker.Instance;
        if (t == null)
        {
            Debug.LogWarning("[WaveUIManager] WaveStatsTracker instance not found!");
            return;
        }

        if (statsKillsText != null)
            statsKillsText.text = $"Kills\n<size=130%><color=#FFD700>{t.Kills}</color></size>";

        if (statsHeadshotsText != null)
            statsHeadshotsText.text = $"Headshots\n<size=130%><color=#FF6B35>{t.Headshots}</color></size>";

        if (statsAccuracyText != null)
            statsAccuracyText.text = $"Accuracy\n<size=130%><color=#00FF88>{t.Accuracy:F1}%</color></size>";
    }

    // ── Populate Right: Next Wave Info ───────────────────────────

    private void PopulateNextWaveInfo()
    {
        var info = NextWaveInfoTracker.Instance;

        if (info == null || info.NextWave == null)
        {
            if (nextWaveTitleText != null) nextWaveTitleText.text = "Final Wave";
            if (nextWaveEnemyListText != null) nextWaveEnemyListText.text = "";
            if (nextWaveDropsText != null) nextWaveDropsText.text = "";
            return;
        }

        // Title + total count
        if (nextWaveTitleText != null)
            nextWaveTitleText.text = $"NEXT: {info.NextWave.waveName}\n" +
                                     $"<size=80%><color=#AAAAAA>{info.TotalEnemies} enemies</color></size>";

        // Enemy type breakdown
        if (nextWaveEnemyListText != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var kv in info.EnemyTypeCounts)
                sb.AppendLine($"<color=#FF4444>▸</color> {kv.Key}  <color=#FFFFFF>×{kv.Value}</color>");
            nextWaveEnemyListText.text = sb.ToString().TrimEnd();
        }

        // Drops
        if (nextWaveDropsText != null)
        {
            string ammoLine = info.FullAmmoRefill
                ? "<color=#FFD700>⬡</color> Ammo  <color=#FFD700>FULL REFILL</color>"
                : info.AmmoRefillAmount > 0
                    ? $"<color=#FFD700>⬡</color> Ammo  <color=#FFD700>+{info.AmmoRefillAmount}</color>"
                    : "<color=#555555>⬡</color> Ammo  <color=#555555>None</color>";

            string healthLine = info.HealthDrop
                ? $"<color=#FF4444>♥</color> Health  <color=#FF4444>{info.HealthDropChance * 100f:F0}% chance</color>"
                : "<color=#555555>♥</color> Health  <color=#555555>None</color>";

            nextWaveDropsText.text = $"{ammoLine}\n{healthLine}";
        }
    }

    // ── Countdown inside panel ───────────────────────────────────

    private IEnumerator BetweenWaveCountdown(float totalSeconds, int nextWaveNumber)
    {
        int remaining = Mathf.CeilToInt(totalSeconds);

        while (remaining > 0)
        {
            ShowPrimary($"Wave {nextWaveNumber} starts in", Color.white);
            ShowSecondary(remaining.ToString(), Color.white);

            // Tick 1 real second, pause-aware
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                if (!isPaused) elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            remaining--;
        }

        ShowPrimary("Get Ready!", Color.yellow);
        ShowSecondary("GO!", Color.yellow);
        yield return new WaitForSecondsRealtime(0.6f);

        HidePrimary();
        HideCountdown();
    }

    // ── Pause / Resume ───────────────────────────────────────────

    private void OnPauseClicked()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseButtonLabel != null) pauseButtonLabel.text = "RESUME";
        Debug.Log("[WaveUIManager] Paused");
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseButtonLabel != null) pauseButtonLabel.text = "PAUSE";
        Debug.Log("[WaveUIManager] Resumed");
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Primary Text ─────────────────────────────────────────────

    public void ShowPrimary(string message, Color color, float duration = 0f)
    {
        if (primaryText == null) return;
        if (primaryCoroutine != null) StopCoroutine(primaryCoroutine);
        primaryText.text = message;
        primaryText.color = color;
        primaryText.gameObject.SetActive(true);
        if (duration > 0f)
            primaryCoroutine = StartCoroutine(HidePrimaryAfter(duration));
    }

    public void HidePrimary()
    {
        if (primaryCoroutine != null) { StopCoroutine(primaryCoroutine); primaryCoroutine = null; }
        if (primaryText != null) primaryText.gameObject.SetActive(false);
    }

    private IEnumerator HidePrimaryAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (primaryText != null) primaryText.gameObject.SetActive(false);
    }

    // ── Secondary Text ───────────────────────────────────────────

    public void ShowSecondary(string message, Color color, float duration = 0f)
    {
        if (secondaryText == null) return;
        if (secondaryCoroutine != null) StopCoroutine(secondaryCoroutine);
        secondaryText.text = message;
        secondaryText.color = color;
        secondaryText.gameObject.SetActive(true);
        if (duration > 0f)
            secondaryCoroutine = StartCoroutine(HideSecondaryAfter(duration));
    }

    public void HideCountdown()
    {
        if (secondaryCoroutine != null) { StopCoroutine(secondaryCoroutine); secondaryCoroutine = null; }
        if (secondaryText != null) secondaryText.gameObject.SetActive(false);
    }

    private IEnumerator HideSecondaryAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (secondaryText != null) secondaryText.gameObject.SetActive(false);
    }

    // ── Wave event wrappers ──────────────────────────────────────

    public void ShowCountdown(int secondsRemaining, int waveNumber)
    {
        ShowPrimary(secondsRemaining > 0 ? $"Wave {waveNumber} starts in" : "", Color.white);
        ShowSecondary(
            secondsRemaining > 0 ? secondsRemaining.ToString() : "Get Ready!",
            secondsRemaining > 0 ? Color.white : Color.yellow
        );
    }

    public void ShowWaveStarted(int waveNumber)
    {
        HideCountdown();
        ShowPrimary($"Wave {waveNumber}", Color.yellow);
    }

    public void ShowWaveCleared(int waveNumber)
    {
        HideWaveNumber();
        ShowPrimary($"Wave {waveNumber}", Color.yellow);
        ShowSecondary("CLEARED!", Color.green, 2f);
    }

    public void ShowVictory()
    {
        HideCountdown();
        HideWaveNumber();
        ShowPrimary("Victory!\nAll waves cleared!", Color.cyan);
        ShowSecondary("CLEARED!", Color.green, 2f);
    }

    public void ShowGameOver(string reason = "")
    {
        HideCountdown();
        HideWaveNumber();
        string msg = string.IsNullOrEmpty(reason) ? "Game Over" : $"Game Over\n{reason}";
        ShowPrimary(msg, Color.red);
        ShowSecondary("OVER", Color.red, 2f);
    }

    public void ClearAll()
    {
        HidePrimary();
        HideCountdown();
        HideWaveNumber();
    }
}
