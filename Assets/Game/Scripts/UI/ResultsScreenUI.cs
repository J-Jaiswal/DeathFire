using TMPro;
using UnityEngine;

public class ResultsScreenUI : MonoBehaviour
{
    [SerializeField] TMP_Text killsText, waveReached, accuracyText, headshotPctText, scoreText;


    private void OnEnable()
    {
        scoreText.text = $"Score: {GameResultsData.Score}";
        killsText.text = $"Kills: {GameResultsData.Kills}";
        waveReached.text = $"WaveReached: {GameResultsData.waveReached}";
        accuracyText.text = $"Accuracy: {GameResultsData.Accuracy:F1}%";
        headshotPctText.text = $"Headshot %: {GameResultsData.HeadshotPercent:F1}%";
    }
}