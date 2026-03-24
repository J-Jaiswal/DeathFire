using UnityEngine;
using System.Collections;
using TMPro;
public class WaveUIDisplay : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TextMeshProUGUI waveLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject victoryPanel;

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

      
    private void Awake() => ClearAll();

    private void HandleWaveStarted(int n)
    {

        if ( n == 11 )
            SetWaveLabel("Final Wave!", Color.yellow);  
        else
            SetWaveLabel($"Wave {n}", Color.yellow);
        
    }
    private void HandleWaveCleared(int n)
    {
        SetWaveLabel($"Wave {n}", Color.white);
        ShowStatus("CLEARED", Color.green, 2f);

    }
  
    private void HandleVictory()
    {
        SetWaveLabel("Victory!", Color.cyan);
        ShowStatus("All waves cleared!", Color.cyan);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StopAllCoroutines();
        victoryPanel?.gameObject.SetActive(true);

    }
    private void HandleGameOver()
    {
        SetWaveLabel("Game Over", Color.red);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        deathPanel?.gameObject.SetActive(true);
        ShowStatus("you Die", Color.red);
    }
    private void HandleCountdown(int seconds, int waveNumber)
    {
        SetWaveLabel($"Wave {waveNumber}", Color.white);
        //ShowStatus(seconds > 0 ? $"{seconds}" : "", Color.white);
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
        deathPanel?.gameObject.SetActive(false);
        victoryPanel?.gameObject.SetActive(false);
        waveLabel?.gameObject.SetActive(false);
        if (statusRoutine != null) { StopCoroutine(statusRoutine); statusRoutine = null; }
        statusLabel?.gameObject.SetActive(false);
    }
}