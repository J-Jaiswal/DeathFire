using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Attach this to a UI GameObject with a TextMeshProUGUI component.
/// Call Show() from your Enemy script on death.
/// The object activates, floats up, fades out, then deactivates itself.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float floatSpeed = 80f;   // pixels per second upward
    [SerializeField] private float lifetime = 1.2f;     // seconds before deactivating

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false); // start hidden
    }

    /// <summary>
    /// Call this from your Enemy on death.
    /// e.g.  floatingText.Show("+100", Color.white);
    /// </summary>
    public void Show(string text, Color color)
    {
        label.text = text;
        label.color = color;

        // Reset position and alpha before activating
        rectTransform.anchoredPosition = Vector2.zero;
        label.color = new Color(color.r, color.g, color.b, 1f);

        gameObject.SetActive(true);
        StartCoroutine(Animate(color));
    }

    private IEnumerator Animate(Color originalColor)
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);

            // Move up
            rectTransform.anchoredPosition = startPos + Vector2.up * (floatSpeed * elapsed);

            // Fade out in second half
            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            label.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        gameObject.SetActive(false);
    }
}