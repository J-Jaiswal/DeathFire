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
    [SerializeField] private TextMeshProUGUI nextWaveTitleText;
    [SerializeField] private TextMeshProUGUI nextWaveEnemyListText;
    [SerializeField] private TextMeshProUGUI nextWaveDropsText;

    [Header("Countdown (inside panel)")]
    [SerializeField] private TextMeshProUGUI countdownLabelText;
    [SerializeField] private TextMeshProUGUI countdownNumberText;

    [Header("Dependencies")]
    [SerializeField] private WavePauseController pauseController;

    public bool IsOpen { get; private set; }

    private void Awake() => panelRoot?.SetActive(false);

    // ── Public Entry Point ───────────────────────────────────────

    /// <summary>WaveManager yields on this. Returns when countdown ends.</summary>
    public IEnumerator Show(int completedWaveNumber, WaveData nextWave, float countdownDuration)
    {
        IsOpen = true;

        PopulateStats(completedWaveNumber);
        PopulateNextWaveInfo(nextWave);
        panelRoot?.SetActive(true);

        WaveStatsTracker.Instance?.LogSummary();

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

    private void PopulateNextWaveInfo(WaveData nextWave)
    {
        var info = NextWaveInfoTracker.Instance;

        if (info == null || info.NextWave == null)
        {
            if (nextWaveTitleText != null) nextWaveTitleText.text = "Final Wave";
            if (nextWaveEnemyListText != null) nextWaveEnemyListText.text = "";
            if (nextWaveDropsText != null) nextWaveDropsText.text = "";
            return;
        }

        if (nextWaveTitleText != null)
            nextWaveTitleText.text = $"NEXT: {info.NextWave.waveName}\n" +
                                     $"<size=80%><color=#AAAAAA>{info.TotalEnemies} enemies</color></size>";

        if (nextWaveEnemyListText != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var kv in info.EnemyTypeCounts)
                sb.AppendLine($"<color=#FF4444>▸</color> {kv.Key}  <color=#FFFFFF>×{kv.Value}</color>");
            nextWaveEnemyListText.text = sb.ToString().TrimEnd();
        }

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