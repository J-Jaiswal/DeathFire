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
    [SerializeField] private FloatingText floatingText;

    private int totalScore = 0;

    public void ReportKill(bool isHeadshot)
    {
        int earned = killScore;

        if (isHeadshot)
        {
            earned += headshotBonus;

            // Prepare the headshot label and trigger via bool
            floatingText?.Prepare("HEADSHOT!", Color.red);
            if (floatingText != null) floatingText.ShowText = true;
        }
        else
        {
            // Prepare the score label and trigger via bool
            floatingText?.Prepare($"+{earned}", Color.white);
            if (floatingText != null) floatingText.ShowText = true;
        }

        AddScore(earned);
    }

    public void ReportWaveCompleted()
    {
        AddScore(waveCompletedBonus);

        floatingText?.Prepare($"+{waveCompletedBonus} WAVE CLEAR!", Color.yellow);
        if (floatingText != null) floatingText.ShowText = true;
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