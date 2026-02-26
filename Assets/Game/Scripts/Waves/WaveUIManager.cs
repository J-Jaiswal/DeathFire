using UnityEngine;
using TMPro;
using System.Collections;

public class WaveUIManager : MonoBehaviour
{
    [Header("Wave Number HUD")]
    [SerializeField] private TextMeshProUGUI waveNumberText;

    [Header("Center Banner")]
    [SerializeField] private GameObject bannerRoot;
    [SerializeField] private TextMeshProUGUI bannerTitleText;
    [SerializeField] private TextMeshProUGUI bannerBodyText;
    [SerializeField] private float bannerDuration = 2.5f;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownRoot;
    [SerializeField] private TextMeshProUGUI countdownText;

    private Coroutine bannerCoroutine;

    private void Start()
    {
        // ✅ Log what's missing so you can fix Inspector assignments
        if (waveNumberText == null) Debug.LogWarning("[WaveUIManager] ⚠️ waveNumberText is NOT assigned!");
        if (bannerRoot == null) Debug.LogWarning("[WaveUIManager] ⚠️ bannerRoot is NOT assigned!");
        if (bannerTitleText == null) Debug.LogWarning("[WaveUIManager] ⚠️ bannerTitleText is NOT assigned!");
        if (bannerBodyText == null) Debug.LogWarning("[WaveUIManager] ⚠️ bannerBodyText is NOT assigned!");
        if (countdownRoot == null) Debug.LogWarning("[WaveUIManager] ⚠️ countdownRoot is NOT assigned!");
        if (countdownText == null) Debug.LogWarning("[WaveUIManager] ⚠️ countdownText is NOT assigned!");

        bannerRoot?.SetActive(false);
        countdownRoot?.SetActive(false);

        Debug.Log("[WaveUIManager] ✅ Start complete.");
    }

    public void ShowWaveStarted(int waveNumber)
    {
        Debug.Log($"[WaveUIManager] 📢 ShowWaveStarted() Wave: {waveNumber}");

        if (waveNumberText != null)
            waveNumberText.text = $"WAVE {waveNumber}";

        HideCountdown();
        ShowBanner($"WAVE {waveNumber}", "SURVIVE!", Color.yellow);
    }

    public void ShowWaveCleared(int waveNumber)
    {
        Debug.Log($"[WaveUIManager] 📢 ShowWaveCleared() Wave: {waveNumber}");
        ShowBanner($"WAVE {waveNumber} CLEAR", "Prepare yourself...", Color.green);
    }

    public void ShowAllWavesCleared()
    {
        Debug.Log("[WaveUIManager] 🏆 ShowAllWavesCleared()");
        ShowBanner("VICTORY", "All waves cleared!", Color.cyan);
        HideCountdown();
    }

    public void ShowCountdown(int secondsRemaining)
    {
        Debug.Log($"[WaveUIManager] ⏱️ ShowCountdown(): {secondsRemaining}");

        if (countdownRoot == null)
        {
            Debug.LogWarning("[WaveUIManager] ⚠️ ShowCountdown() skipped — countdownRoot is null!");
            return;
        }

        countdownRoot.SetActive(true);

        if (countdownText != null)
            countdownText.text = secondsRemaining > 0
                ? $"Next wave in {secondsRemaining}..."
                : "GET READY!";
    }

    public void HideCountdown()
    {
        Debug.Log("[WaveUIManager] 🙈 HideCountdown()");
        countdownRoot?.SetActive(false);
    }

    private void ShowBanner(string title, string body, Color color)
    {
        if (bannerRoot == null)
        {
            Debug.LogWarning($"[WaveUIManager] ⚠️ ShowBanner() skipped — bannerRoot is null! Title was: '{title}'");
            return;
        }

        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(BannerRoutine(title, body, color));
    }

    private IEnumerator BannerRoutine(string title, string body, Color color)
    {
        Debug.Log($"[WaveUIManager] 🎬 BannerRoutine started | Title: '{title}' | Body: '{body}'");

        bannerRoot.SetActive(true);

        if (bannerTitleText != null) { bannerTitleText.text = title; bannerTitleText.color = color; }
        if (bannerBodyText != null) { bannerBodyText.text = body; bannerBodyText.color = color; }

        yield return new WaitForSeconds(bannerDuration);

        bannerRoot.SetActive(false);
        Debug.Log($"[WaveUIManager] 🎬 BannerRoutine ended | '{title}' hidden.");
    }
}