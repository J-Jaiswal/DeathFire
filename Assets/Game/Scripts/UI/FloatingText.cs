using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Two instances recommended in the Canvas:
///   • "KillFloatingText"  — for kill / headshot scores
///   • "ComboFloatingText" — for combo / wave bonuses
///
/// Each instance floats up from Vector2.zero (its anchored home),
/// fades out, then deactivates. A random horizontal + vertical
/// spawn offset (±offsetRange) keeps overlapping texts readable.
/// Trigger via the ShowText bool or call Show() directly.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI label;

    [Header("Animation")]
    [SerializeField] private float floatSpeed = 80f;
    [SerializeField] private float lifetime = 1.2f;

    [Header("Random Spawn Offset")]
    [Tooltip("Max pixels randomly added/subtracted on X axis at spawn.")]
    [SerializeField] private float offsetRangeX = 50f;
    [Tooltip("Max pixels randomly added/subtracted on Y axis at spawn.")]
    [SerializeField] private float offsetRangeY = 60f;

    // ── Backing field — NOT serialized so Inspector can't leave it true ──
    private bool showText;
    public bool ShowText
    {
        get => showText;
        set
        {
            showText = value;
            if (showText) TriggerFromBool();
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

    private void TriggerFromBool() => Show(pendingText, pendingColor);

    /// <summary>Set text + colour; arm ShowText to fire Show().</summary>
    public void Prepare(string text, Color color)
    {
        pendingText = text;
        pendingColor = color;
    }

    /// <summary>Direct call — activates and runs the float animation.</summary>
    public void Show(string text, Color color)
    {
        label.text = text;

        gameObject.SetActive(true);

        // Apply random spawn offset so stacked texts don't perfectly overlap
        float rx = Random.Range(-offsetRangeX, offsetRangeX);
        float ry = Random.Range(-offsetRangeY, offsetRangeY);
        rectTransform.anchoredPosition = new Vector2(rx, ry);

        label.color = new Color(color.r, color.g, color.b, 1f);

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Animate(color));
    }

    private IEnumerator Animate(Color originalColor)
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;   // captured after offset applied

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);

            rectTransform.anchoredPosition = startPos + Vector2.up * (floatSpeed * elapsed);

            // Fade out in the second half
            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            label.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        gameObject.SetActive(false);
        showText = false;   // reset backing field — avoids re-triggering setter
        activeCoroutine = null;
    }
}