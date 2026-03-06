using UnityEngine;
using System.Collections;
using TMPro;

public class WaveUIDisplay : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TextMeshProUGUI waveLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;

    private Coroutine statusRoutine;
    [SerializeField] private TextMeshProUGUI primaryText;
    [SerializeField] private TextMeshProUGUI secondaryText;
    [SerializeField] private TextMeshProUGUI waveNumberText;

    private Coroutine primaryRoutine;
    private Coroutine secondaryRoutine;

    // ── Lifecycle ────────────────────────────────────────────────

    private void OnEnable()
    {
        WaveEventBus.OnWaveStarted += HandleWaveStarted;
        WaveEventBus.OnWaveCleared += HandleWaveCleared;
        WaveEventBus.OnVictory += HandleVictory;
        WaveEventBus.OnGameOver += HandleGameOver;
        WaveEventBus.OnCountdownTick += HandleCountdown;
    }

    private void OnDisable()
    {
        WaveEventBus.OnWaveStarted -= HandleWaveStarted;
        WaveEventBus.OnWaveCleared -= HandleWaveCleared;
        WaveEventBus.OnVictory -= HandleVictory;
        WaveEventBus.OnGameOver -= HandleGameOver;
        WaveEventBus.OnCountdownTick -= HandleCountdown;
    }

    private void Start() => ClearAll();

    // ── Event Handlers ───────────────────────────────────────────

    private void HandleWaveStarted(int n)
    {
        SetWaveLabel($"Wave {n}", Color.yellow);
        ShowStatus("Incoming!", Color.yellow, 2f);
        SetWaveNumber(n);
        HideSecondary();
        ShowPrimary($"Wave {n}", Color.yellow, 2f);
    }

    private void HandleWaveCleared(int n)
    {
        SetWaveLabel($"Wave {n}", Color.white);
        ShowStatus("Cleared!", Color.green, 2f);
        HideWaveNumber();
        ShowPrimary($"Wave {n}", Color.yellow);
        ShowSecondary("CLEARED!", Color.green, 2f);
    }

    private void HandleVictory()
    {
        SetWaveLabel("Victory!", Color.cyan);
        ShowStatus("All waves cleared!", Color.cyan);
        ClearAll();
        ShowPrimary("Victory!\nAll waves cleared!", Color.cyan);
    }

    private void HandleGameOver(string reason)
    {
        SetWaveLabel("Game Over", Color.red);
        string msg = string.IsNullOrEmpty(reason) ? "Better luck next time" : reason;
        ShowStatus(msg, Color.red);
        ClearAll();
        string msg = string.IsNullOrEmpty(reason) ? "Game Over" : $"Game Over\n{reason}";
        ShowPrimary(msg, Color.red);
        ShowSecondary("OVER", Color.red, 2f);
    }

    private void HandleCountdown(int seconds, int waveNumber)
    {
        SetWaveLabel($"Wave {waveNumber}", Color.white);
        ShowStatus(seconds > 0 ? $"Starting in {seconds}..." : "Get Ready!", Color.white);
    }

    // ── Wave Label ───────────────────────────────────────────────

    private void SetWaveLabel(string msg, Color color)
    {
        if (waveLabel == null) return;
        waveLabel.text = msg;
        waveLabel.color = color;
        waveLabel.gameObject.SetActive(true);
    }

    // ── Status Label ─────────────────────────────────────────────

    private void ShowStatus(string msg, Color color, float duration = 0f)
    {
        if (statusLabel == null) return;
        if (statusRoutine != null) StopCoroutine(statusRoutine);
        statusLabel.text = msg;
        statusLabel.color = color;
        statusLabel.gameObject.SetActive(true);
        if (duration > 0f)
            statusRoutine = StartCoroutine(HideStatusAfter(duration));
    }

    private IEnumerator HideStatusAfter(float delay)
    {
        // Use unscaled time — status must show even if timeScale is 0
        yield return new WaitForSecondsRealtime(delay);
        statusLabel?.gameObject.SetActive(false);
        statusRoutine = null;
        if (seconds > 0)
        {
            ShowPrimary($"Wave {waveNumber} starts in", Color.white);
            ShowSecondary(seconds.ToString(), Color.white);
        }
        else
        {
            HidePrimary();
            ShowSecondary("Get Ready!", Color.yellow);
        }
    }

    // ── Wave Number ──────────────────────────────────────────────

    public void SetWaveNumber(int n)
    {
        if (waveNumberText == null) return;
        waveNumberText.text = $"Wave {n}";
        waveNumberText.gameObject.SetActive(true);
    }

    public void HideWaveNumber()
    {
        if (waveNumberText != null)
            waveNumberText.gameObject.SetActive(false);
    }

    // ── Primary ──────────────────────────────────────────────────

    public void ShowPrimary(string msg, Color color, float duration = 0f)
    {
        if (primaryText == null) return;
        if (primaryRoutine != null) StopCoroutine(primaryRoutine);
        primaryText.text = msg;
        primaryText.color = color;
        primaryText.gameObject.SetActive(true);
        if (duration > 0f)
            primaryRoutine = StartCoroutine(HideAfter(primaryText, duration, () => primaryRoutine = null));
    }

    public void HidePrimary()
    {
        if (primaryRoutine != null) { StopCoroutine(primaryRoutine); primaryRoutine = null; }
        primaryText?.gameObject.SetActive(false);
    }

    // ── Secondary ────────────────────────────────────────────────

    public void ShowSecondary(string msg, Color color, float duration = 0f)
    {
        if (secondaryText == null) return;
        if (secondaryRoutine != null) StopCoroutine(secondaryRoutine);
        secondaryText.text = msg;
        secondaryText.color = color;
        secondaryText.gameObject.SetActive(true);
        if (duration > 0f)
            secondaryRoutine = StartCoroutine(HideAfter(secondaryText, duration, () => secondaryRoutine = null));
    }

    public void HideSecondary()
    {
        if (secondaryRoutine != null) { StopCoroutine(secondaryRoutine); secondaryRoutine = null; }
        secondaryText?.gameObject.SetActive(false);
    }

    // ── Helpers ──────────────────────────────────────────────────

    public void ClearAll()
    {
        waveLabel?.gameObject.SetActive(false);
        if (statusRoutine != null) { StopCoroutine(statusRoutine); statusRoutine = null; }
        statusLabel?.gameObject.SetActive(false);
    public void ClearAll() { HidePrimary(); HideSecondary(); HideWaveNumber(); }

    private IEnumerator HideAfter(TextMeshProUGUI target, float delay, System.Action onDone)
    {
        yield return new WaitForSeconds(delay);
        target?.gameObject.SetActive(false);
        onDone?.Invoke();
    }
}