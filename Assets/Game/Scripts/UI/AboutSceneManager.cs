using UnityEngine;
using UnityEngine.UI;

public class AboutSceneManager : MonoBehaviour
{
    public GameObject gameplayPanel;
    public GameObject enemiesPanel;
    public GameObject controlsPanel;

    public Button gameplayBtn;
    public Button enemiesBtn;
    public Button controlsBtn;

    void Start()
    {
        gameplayBtn.onClick.AddListener(() => ShowPanel(0));
        enemiesBtn.onClick.AddListener(() => ShowPanel(1));
        controlsBtn.onClick.AddListener(() => ShowPanel(2));
        ShowPanel(0);
    }

    void ShowPanel(int index)
    {
        gameplayPanel.SetActive(index == 0);
        enemiesPanel.SetActive(index == 1);
        controlsPanel.SetActive(index == 2);
    }
}