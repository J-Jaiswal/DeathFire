using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class AmmoSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject reloadPanel;   // "RELOADING..." text
    [SerializeField] private Image reloadBarFill; // progress bar fill

    // Read by WeaponAnimationDriver to pick the right reload animation
    public bool IsEmpty => currentAmmo == 0;
    public bool IsReloading => isReloading;

    private int currentAmmo;
    private bool isReloading;
    private Coroutine reloadCoroutine;

    private void Start()
    {
        currentAmmo = magazineSize;
        reloadPanel?.SetActive(false);
        RefreshUI();
    }

    // ── Called by WeaponAnimationDriver ──────────────────────────────────────

    /// <summary>
    /// Returns true if a shot is allowed and decrements ammo.
    /// Returns false if empty or already reloading.
    /// </summary>
    public bool TryShoot()
    {
        if (isReloading || currentAmmo <= 0) return false;

        currentAmmo--;
        RefreshUI();
        return true;
    }

    /// <summary>Returns true if a reload would actually do something.</summary>
    public bool NeedsReload() => !isReloading && currentAmmo < magazineSize;

    /// <summary>
    /// Starts the reload timer. Calls onComplete when finished so
    /// WeaponAnimationDriver can clear its isReloading flag.
    /// </summary>
    public void StartReload(Action onComplete)
    {
        if (isReloading) return;

        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = StartCoroutine(ReloadRoutine(onComplete));
    }

    // ── Internal ─────────────────────────────────────────────────────────────

    private IEnumerator ReloadRoutine(Action onComplete)
    {
        isReloading = true;
        reloadPanel?.SetActive(true);

        float elapsed = 0f;
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            if (reloadBarFill != null)
                reloadBarFill.fillAmount = elapsed / reloadTime;
            yield return null;
        }

        currentAmmo = magazineSize;
        isReloading = false;

        reloadPanel?.SetActive(false);
        if (reloadBarFill != null) reloadBarFill.fillAmount = 0f;

        RefreshUI();
        onComplete?.Invoke(); // tell WeaponAnimationDriver we're done
    }

    private void RefreshUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {magazineSize}";
    }
}