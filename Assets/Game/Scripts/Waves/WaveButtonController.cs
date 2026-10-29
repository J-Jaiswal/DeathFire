using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveButtonController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI pauseButtonLabel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Escape Key")]
    [Tooltip("If true, pressing Escape during a wave loads the main menu.")]
    [SerializeField] private bool escapeLoadsMenu = true;

    public bool IsPaused { get; private set; }

    //  Unity ------------->

    private void Update()
    {
        if (!escapeLoadsMenu) return;
        if (Input.GetKeyDown(KeyCode.Escape))
            GoToMainMenu();
    }

    //  Panel Buttons -------------->

    public void Toggle() {
        UISoundManager.Instance.PlayButtonClick();
        if (IsPaused) Resume(); else Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseButtonLabel != null) pauseButtonLabel.text = "RESUME";
        Debug.Log("[WavePauseController] Paused");
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pauseButtonLabel != null) pauseButtonLabel.text = "PAUSE";
        Debug.Log("[WavePauseController] Resumed");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UISoundManager.Instance?.PlayButtonClick();

        // Restore cursor so the main menu is fully interactive.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Use SceneLoader for a proper loading-screen transition if available.
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(mainMenuSceneName);
        else
            SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ButtonHover()
    {
        UISoundManager.Instance?.PlayButtonHover();
    }

 
    public void QuitGame()
    {
        UISoundManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_WEBGL
        // Application.Quit() does nothing in WebGL builds.
        // Show a native browser confirm-dialog; if the user confirms, close the tab.
        Application.ExternalEval(
            "if(confirm('Leave the game?')){window.close();" +
            // window.close() only works when the tab was opened by script;
            // as a fallback navigate to a "thanks for playing" page.
            "if(!window.closed){window.location.href='about:blank';}}");
#else
        Application.Quit();
#endif
    }
}