using UnityEngine;
using UnityEngine.SceneManagement;  
public class UIButtonHandler : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameScene = "Game";
    public string aboutScene = "About";

    // ─── Scene Navigation ─────────────────────────────────────────

    public void PlayGame() { PlayClick(); SceneLoader.Instance.LoadScene(gameScene); }
    public void LoadAbout() { PlayClick(); SceneManager.LoadScene(aboutScene); }
    public void LoadMainMenu() { PlayClick(); SceneManager.LoadScene(mainMenuScene); }
    public void Restart() { PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            PlayClick();
            SceneLoader.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("UIButtonHandler: Scene name is empty!");
        }
    }

    // ─── Application ──────────────────────────────────────────────

    public void QuitGame()
    {
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("UIButtonHandler: Game Quit.");
    }

    // ─── Pause ────────────────────────────────────────────────────

    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
    public void TogglePause() => Time.timeScale = Time.timeScale == 0f ? 1f : 0f;

    // ─── UI Sound ─────────────────────────────────────────────────

    public void ButtonHover() => UISoundManager.Instance?.PlayButtonHover();
    void PlayClick() => UISoundManager.Instance?.PlayButtonClick();
}