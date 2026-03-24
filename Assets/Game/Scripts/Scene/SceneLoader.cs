using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    [Header("Bar Speed")]
    [Range(0.1f, 1f)]
    public float barSpeed = 0.4f;   // controls how fast bar goes 0 → 90%

    bool sceneReady = false;

    void Awake()
    {
            Instance = this;
   
    }

    public void OnSceneReady() => sceneReady = true;

    public void LoadScene(string sceneName) => StartCoroutine(LoadAsync(sceneName));

    //public void RestartScene()
    //{
    //    Time.timeScale = 1f;
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        
    //}

    IEnumerator LoadAsync(string sceneName)
    {
        // ── 1. Prepare ────────────────────────────────────────────
        sceneReady = false;
        Time.timeScale = 1f;
        loadingPanel?.SetActive(true);
        SetProgress(0f);
        yield return new WaitForSecondsRealtime(0.1f);

        // ── 2. Start Unity loading silently in background ─────────
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;   // don't switch scene yet

        // ── 3. Animate bar 0% → 90% (regardless of load speed) ───
        float display = 0f;
        while (display < 0.9f)
        {
            display += Time.unscaledDeltaTime * barSpeed;
            display = Mathf.Min(display, 0.9f);
            SetProgress(display);
            yield return null;
        }

        // ── 4. Bar reached 90% — now check if Unity is done ───────
        SetProgress(0.9f);
        if (loadingText != null) loadingText.text = "Loading... 90%";

        while (op.progress < 0.9f)   // wait if Unity is still loading
        {
            yield return null;
        }

        // ── 5. Unity is ready — animate 90% → 100% then activate ──
        while (display < 1f)
        {
            display += Time.unscaledDeltaTime * barSpeed;
            display = Mathf.Min(display, 1f);
            SetProgress(display);
            yield return null;
        }

        SetProgress(1f);
        op.allowSceneActivation = true;   // scene switches here
        yield return null;

        // ── 6. Wait for SceneReadySignal ──────────────────────────
        float elapsed = 0f;
        while (!sceneReady && elapsed < 5f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // ── 7. Done ───────────────────────────────────────────────
        if (loadingText != null) loadingText.text = "Ready!";
        yield return new WaitForSecondsRealtime(0.4f);
        loadingPanel?.SetActive(false);
    }

    void SetProgress(float value)
    {
        if (loadingBar != null) loadingBar.value = value;
        if (loadingText != null) loadingText.text = $"Loading... {Mathf.RoundToInt(value * 100)}%";
    }
}
