using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Animation Settings")]
    [SerializeField] private float punchScale = 1.6f;
    [SerializeField] private float scaleUpDuration = 0.15f;
    [SerializeField] private float scaleDownDuration = 0.7f;

    [Header("Colors")]
    [SerializeField] private Color color321 = Color.white;
    [SerializeField] private Color colorGo = Color.green;

    private Coroutine animCoroutine;

    private void OnEnable() => WaveEventBus.OnCountdownTick += HandleCountdownTick;
    private void OnDisable() => WaveEventBus.OnCountdownTick -= HandleCountdownTick;

    private void HandleCountdownTick(int remaining, int waveNumber)
    {
        PlayTick(remaining);
    }

    public void PlayTick(int remaining)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        countdownText.text = remaining > 0 ? remaining.ToString() : "GO";
        countdownText.color = remaining > 0 ? color321 : colorGo;

        animCoroutine = StartCoroutine(PunchAndShrink());
    }

    private IEnumerator PunchAndShrink()
    {
        countdownText.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scaleUpDuration;
            float scale = Mathf.Lerp(0f, punchScale, EaseOutBack(t));
            countdownText.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scaleDownDuration;
            float scale = Mathf.Lerp(punchScale, 0f, EaseInCubic(t));
            countdownText.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.zero;
        countdownText.gameObject.SetActive(false);
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private float EaseInCubic(float t) => t * t * t;
}