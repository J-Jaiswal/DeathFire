using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// All wave-related UI in one place.
/// WaveManager calls methods on this directly.
/// </summary>
public class WaveUIManager : MonoBehaviour
{
    [Header("Wave Number HUD")]
    [SerializeField] private TextMeshProUGUI waveNumberText;   // persistent top-left etc.

    [Header("Center Banner")]
    [SerializeField] private GameObject bannerRoot;       // panel to show/hide
    [SerializeField] private TextMeshProUGUI bannerTitleText;
    [SerializeField] private TextMeshProUGUI bannerBodyText;
    [SerializeField] private float bannerDuration = 2.5f;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownRoot;
    [SerializeField] private TextMeshProUGUI countdownText;

    private Coroutine bannerCoroutine;

    private void Start()
    {
        bannerRoot?.SetActive(false);
        countdownRoot?.SetActive(false);
    }

    // ── Called by WaveManager ─────────────────────────────────────────────────

    public void ShowWaveStarted(int waveNumber)
    {
        if (waveNumberText != null)
            waveNumberText.text = $"WAVE {waveNumber}";

        HideCountdown();
        ShowBanner($"WAVE {waveNumber}", "SURVIVE!", Color.yellow);
    }

    public void ShowWaveCleared(int waveNumber)
    {
        ShowBanner($"WAVE {waveNumber} CLEAR", "Prepare yourself...", Color.green);
    }

    public void ShowAllWavesCleared()
    {
        ShowBanner("VICTORY", "All waves cleared!", Color.cyan);
        HideCountdown();
    }

    public void ShowCountdown(int secondsRemaining)
    {
        if (countdownRoot == null) return;

        countdownRoot.SetActive(true);

        if (countdownText != null)
            countdownText.text = secondsRemaining > 0
                ? $"Next wave in  {secondsRemaining}..."
                : "GET READY!";
    }

    public void HideCountdown()
    {
        countdownRoot?.SetActive(false);
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    private void ShowBanner(string title, string body, Color color)
    {
        if (bannerRoot == null) return;

        // Cancel previous banner if still showing
        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(BannerRoutine(title, body, color));
    }

    private IEnumerator BannerRoutine(string title, string body, Color color)
    {
        bannerRoot.SetActive(true);

        if (bannerTitleText != null) { bannerTitleText.text = title; bannerTitleText.color = color; }
        if (bannerBodyText != null) { bannerBodyText.text = body; bannerBodyText.color = color; }

        yield return new WaitForSeconds(bannerDuration);

        bannerRoot.SetActive(false);
    }
}