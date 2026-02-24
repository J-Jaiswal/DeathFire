using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int killScore = 100;
    [SerializeField] private int headshotBonus = 50;
    [SerializeField] private int waveCompletedBonus = 500;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("References")]
    [SerializeField] private FloatingText floatingText; // replaced FloatingTextPool

    private int totalScore = 0;

    public void ReportKill(bool isHeadshot)
    {
        int earned = killScore;

        if (isHeadshot)
        {
            earned += headshotBonus;
            floatingText?.Show("HEADSHOT!", Color.red);
        }

        AddScore(earned);
        floatingText?.Show($"+{earned}", Color.white);
    }

    public void ReportWaveCompleted()
    {
        AddScore(waveCompletedBonus);
    }

    private void AddScore(int amount)
    {
        totalScore += amount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = totalScore.ToString("N0");
    }
}