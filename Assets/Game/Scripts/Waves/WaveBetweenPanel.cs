using UnityEngine;
using System.Collections;
using TMPro;

public class WaveBetweenPanel : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Left — Wave Stats")]
    [SerializeField] private TextMeshProUGUI statsHeaderText;
    [SerializeField] private TextMeshProUGUI statsKillsText;
    [SerializeField] private TextMeshProUGUI statsHeadshotsText;
    [SerializeField] private TextMeshProUGUI statsAccuracyText;

    [Header("Right — Next Wave Info")]
    [SerializeField] private TextMeshProUGUI nextWaveHeader;

    [SerializeField] private TextMeshProUGUI nextWaveEnemies;
    [SerializeField] private TextMeshProUGUI nextWaveEnemyNum;
    [SerializeField] private TextMeshProUGUI nextWaveammo;

    [Header("Countdown (inside panel)")]
    [SerializeField] private TextMeshProUGUI countdownLabelText;
    [SerializeField] private TextMeshProUGUI countdownNumberText;

    [Header("Dependencies")]
    [SerializeField] private WavePauseController pauseController;

    public bool IsOpen { get; private set; }

    private void Awake() => panelRoot?.SetActive(false);
    private int currentWaveNumber;

    // ── Public Entry Point ───────────────────────────────────────

    /// <summary>WaveManager yields on this. Returns when countdown ends.</summary>
    public IEnumerator Show(int completedWaveNumber, WaveData nextWave, float countdownDuration)
    {
        IsOpen = true;
        currentWaveNumber = completedWaveNumber;    
        PopulateStats(currentWaveNumber);
        PopulateNextWaveInfo(nextWave , currentWaveNumber);
        panelRoot?.SetActive(true);

        //WaveStatsTracker.Instance?.LogSummary();

        yield return StartCoroutine(PanelCountdown(countdownDuration, completedWaveNumber + 1));

        Close();
    }

    private void Close()
    {
        panelRoot?.SetActive(false);
        IsOpen = false;
        pauseController?.Resume();
    }

    // ── Stats ────────────────────────────────────────────────────

    private void PopulateStats(int waveNumber)
    {
        if (statsHeaderText != null)
            statsHeaderText.text = $"WAVE {waveNumber} STATS";

        var t = WaveStatsTracker.Instance;
        if (t == null)
        {
            Debug.LogWarning("[WaveBetweenPanel] WaveStatsTracker not found!");
            return;
        }

        if (statsKillsText != null)
            statsKillsText.text =
                $"Kills\n<size=130%><color=#FFD700>{t.Kills}</color></size>";

        if (statsHeadshotsText != null)
            statsHeadshotsText.text =
                $"Headshots\n<size=130%><color=#FF6B35>{t.Headshots}</color></size>";

        if (statsAccuracyText != null)
            statsAccuracyText.text =
                $"Accuracy\n<size=130%><color=#00FF88>{t.Accuracy:F1}%</color></size>";
    }

    // ── Next Wave Info ───────────────────────────────────────────

    private void PopulateNextWaveInfo(WaveData nextWave ,  int waveNumber)
    {
        var info = NextWaveInfoTracker.Instance;
        if (info == null || info.NextWave == null)
        {  
            return;
        }

        if (nextWaveHeader != null )
            nextWaveHeader.text = $"WAVE {waveNumber +1 } INFO";
        if (nextWaveEnemies != null)
            nextWaveEnemies.text =
                $"Enemies Details\n<size=100%><color=#FFD700>{info.EnemyDetails}</color></size>";

        if (nextWaveEnemyNum != null)
            nextWaveEnemyNum.text =
                $"Total Enemies\n<size=110%><color=#FFD700>{info.TotalEnemies}</color></size>";

        string ammoLine = info.FullAmmoRefill
                ? " Ammo\n<size=110%><color=#FFD700>FULL REFILL</color></size>"
                : info.AmmoRefillAmount > 0
                    ? $"<color=#FFD700>⬡</color> Ammo\n<size=110%><color=#FFD700>+{info.AmmoRefillAmount}</color></size>"
                    : "<color=#555555>⬡</color> Ammo\n<size=110%><color=#FFD700>None</color></size>";

        nextWaveammo.text = $"{ammoLine}\n"; 

    }
    // ── Countdown ────────────────────────────────────────────────

    private IEnumerator PanelCountdown(float totalSeconds, int nextWaveNumber)
    {
        int remaining = Mathf.CeilToInt(totalSeconds);

        while (remaining > 0)
        {
            SetCountdownDisplay($"Wave {nextWaveNumber} starts in", remaining.ToString(), Color.white);

            float elapsed = 0f;
            while (elapsed < 1f)
            {
                if (pauseController == null || !pauseController.IsPaused)
                    elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            remaining--;
        }

        SetCountdownDisplay("Get Ready!", "GO!", Color.yellow);
        yield return new WaitForSecondsRealtime(0.6f);
    }

    private void SetCountdownDisplay(string label, string number, Color color)
    {
        if (countdownLabelText != null) { countdownLabelText.text = label; countdownLabelText.color = color; }
        if (countdownNumberText != null) { countdownNumberText.text = number; countdownNumberText.color = color; }
    }
}