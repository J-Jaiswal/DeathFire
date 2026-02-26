using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Attach this to a UI GameObject with a TextMeshProUGUI component.
/// Trigger via the ShowText bool property, or call Show() directly.
/// The object activates, floats up, fades out, then deactivates and resets the bool.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float floatSpeed = 80f;
    [SerializeField] private float lifetime = 1.2f;

    // FIX 1: Do NOT serialize showText — prevents Inspector from accidentally leaving it true
    // which would fire before Awake sets the object inactive.
    private bool showText;
    public bool ShowText
    {
        get => showText;
        set
        {
            showText = value;
            if (showText)
                TriggerFromBool();
        }
    }

    private string pendingText = "+100";
    private Color pendingColor = Color.white;

    private RectTransform rectTransform;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void TriggerFromBool()
    {
        Show(pendingText, pendingColor);
    }

    /// <summary>
    /// Set text + colour, then arm the bool so Show fires.
    /// </summary>
    public void Prepare(string text, Color color)
    {
        pendingText = text;
        pendingColor = color;
    }

    /// <summary>
    /// Direct call — activates the object and runs the float animation.
    /// </summary>
    public void Show(string text, Color color)
    {
        label.text = text;

        // FIX 2: Assign anchoredPosition AFTER SetActive so layout is valid,
        // and use Vector2.zero directly rather than reading a potentially stale value.
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = Vector2.zero;
        label.color = new Color(color.r, color.g, color.b, 1f);

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(Animate(color));
    }

    private IEnumerator Animate(Color originalColor)
    {
        float elapsed = 0f;
        // FIX 2: Capture start position after the object is active and position is set.
        Vector2 startPos = rectTransform.anchoredPosition;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);

            rectTransform.anchoredPosition = startPos + Vector2.up * (floatSpeed * elapsed);

            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            label.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Reset everything
        gameObject.SetActive(false);
        showText = false;  // reset backing field without re-triggering the setter
        activeCoroutine = null;
    }
}