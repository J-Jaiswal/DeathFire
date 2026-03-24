using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveButtonController : MonoBehaviour
{
   
    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI pauseButtonLabel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

   
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
        UISoundManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ButtonHover()
    {
        UISoundManager.Instance.PlayButtonHover();
    }

    public  void QuitGame()
    {
        UISoundManager.Instance.PlayButtonClick();

        Time.timeScale = 1f;
     #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
     #else
        Application.Quit();
     #endif
    }
}