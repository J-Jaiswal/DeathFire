using UnityEngine;
using System.Collections;
using TMPro;
public class WaveUIDisplay : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TextMeshProUGUI waveLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;
    private Coroutine statusRoutine;

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

    private void HandleWaveStarted(int n)
    {
        SetWaveLabel($"Wave {n}", Color.yellow);
    }
    private void HandleWaveCleared(int n)
    {
        SetWaveLabel($"Wave {n}", Color.white);
        ShowStatus("Cleared!", Color.green, 2f);
    }
    private void HandleVictory()
    {
        SetWaveLabel("Victory!", Color.cyan);
        ShowStatus("All waves cleared!", Color.cyan);
    }
    private void HandleGameOver(string reason)
    {
        SetWaveLabel("Game Over", Color.red);
        string msg = string.IsNullOrEmpty(reason) ? "Better luck next time" : reason;
        ShowStatus(msg, Color.red);
    }
    private void HandleCountdown(int seconds, int waveNumber)
    {
        SetWaveLabel($"Wave {waveNumber}", Color.white);
        ShowStatus(seconds > 0 ? $"{seconds}" : "", Color.white);
    }

    private void SetWaveLabel(string msg, Color color)
    {
        if (waveLabel == null) return;
        waveLabel.text = msg;
        waveLabel.color = color;
        waveLabel.gameObject.SetActive(true);
    }

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
        yield return new WaitForSecondsRealtime(delay);
        statusLabel?.gameObject.SetActive(false);
        statusRoutine = null;
    }

    public void ClearAll()
    {
        waveLabel?.gameObject.SetActive(false);
        if (statusRoutine != null) { StopCoroutine(statusRoutine); statusRoutine = null; }
        statusLabel?.gameObject.SetActive(false);
    }
}