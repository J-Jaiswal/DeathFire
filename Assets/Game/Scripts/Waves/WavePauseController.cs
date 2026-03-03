using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WavePauseController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseButtonLabel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

    private void Start()
    {
        pauseButton?.onClick.AddListener(Toggle);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
        quitButton?.onClick.AddListener(Quit);
    }

    public void Toggle() { if (IsPaused) Resume(); else Pause(); }

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

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}