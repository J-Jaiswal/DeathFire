using UnityEngine;
using TMPro;
using System.Collections;

public class WaveUIManager : MonoBehaviour
{
    [Header("Two UI Texts")]
    [SerializeField] private TextMeshProUGUI primaryText;   // wave label — stays during wave
    [SerializeField] private TextMeshProUGUI secondaryText; // big text: numbers, CLEARED!, GO!

    private Coroutine primaryCoroutine;
    private Coroutine secondaryCoroutine;

    private void Start()
    {
        if (primaryText == null) Debug.LogWarning("[WaveUIManager]  primaryText not assigned!");
        if (secondaryText == null) Debug.LogWarning("[WaveUIManager]  secondaryText not assigned!");
        ClearAll();
    }

   

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

    // ─── Convenience wrappers ─────────────────────────────────────────────────

    
    public void ShowCountdown(int secondsRemaining, int waveNumber)
    {
        ShowPrimary(
            secondsRemaining > 0 ? $"Wave {waveNumber} starts In " : "",
            Color.white
        );
        ShowSecondary(
            secondsRemaining > 0 ? secondsRemaining.ToString() : "Get Ready!",
            secondsRemaining > 0 ? Color.white : Color.yellow
        );
    }

    // Wave is now live — primary stays visible the whole wave, secondary hides
    public void ShowWaveStarted(int waveNumber)
    {
        HideCountdown();
        ShowPrimary($"Wave {waveNumber}", Color.yellow);
    }

    // Wave cleared — primary keeps showing wave number, secondary flashes CLEARED! for 2s
    // WaveManager should wait for this 2s before starting the next countdown
    public void ShowWaveCleared(int waveNumber)
    {
        ShowPrimary($"Wave {waveNumber}", Color.yellow);
        ShowSecondary("CLEARED!", Color.green, 2f);
    }

    public void ShowVictory()
    {
        HideCountdown();
        ShowPrimary("Victory!\nAll waves cleared!", Color.cyan);
        ShowSecondary("CLEARED!", Color.green, 2f);
    }

    public void ShowGameOver(string reason = "")
    {
        HideCountdown();
        string msg = string.IsNullOrEmpty(reason) ? "Game Over" : $"Game Over\n{reason}";
        ShowPrimary(msg, Color.red);
        ShowSecondary("OVER", Color.red, 2f);
    }

    public void ClearAll()
    {
        HidePrimary();
        HideCountdown();
    }
}