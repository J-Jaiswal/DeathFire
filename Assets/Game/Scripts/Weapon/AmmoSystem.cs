using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class AmmoSystem : MonoBehaviour
{
    public static AmmoSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int totalReserveAmmo = 36;
    [SerializeField] private float reloadTime = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadingText;
    [SerializeField] private TextMeshProUGUI ammoRefillNoticeText; // optional — shows "+X Ammo" on refill

    [Header("Death / No Ammo Panel")]
    [Tooltip("Panel shown when both magazine AND reserve are empty.")]
    [SerializeField] private GameObject noAmmoDeathPanel;

    public bool IsEmpty => currentAmmo == 0;
    public bool IsReloading => isReloading;

    private int currentAmmo;
    private int reserveAmmo;
    private bool isReloading;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        currentAmmo = magazineSize;
        reserveAmmo = totalReserveAmmo;

        if (reloadingText != null) reloadingText.gameObject.SetActive(false);
        if (noAmmoDeathPanel != null) noAmmoDeathPanel.SetActive(false);
        if (ammoRefillNoticeText != null) ammoRefillNoticeText.gameObject.SetActive(false);

        RefreshUI();
    }

    // ─────────────────────────────────────────────
    //  Wave Ammo Refill — call this at wave start
    // ─────────────────────────────────────────────

    /// <summary>
    /// Call this at the start of each wave, passing the incoming WaveData.
    /// Handles both full refill and partial refill transparently.
    /// </summary>
    public void ApplyWaveAmmoRefill(WaveData wave)
    {
        if (wave == null) return;

        if (wave.fullAmmoRefill)
        {
            // Refill magazine to full AND reset reserve to its default
            int magazineAdded = magazineSize - currentAmmo;
            int reserveAdded = totalReserveAmmo - reserveAmmo;

            currentAmmo = magazineSize;
            reserveAmmo = totalReserveAmmo;

            int totalAdded = magazineAdded + reserveAdded;
            if (totalAdded > 0)
                ShowRefillNotice($"+{totalAdded} AMMO");

            Debug.Log($"[AmmoSystem] Full ammo refill applied for wave: {wave.waveName}");
        }
        else if (wave.ammoRefillAmount > 0)
        {
            // Add to reserve only — player keeps their current magazine mid-wave transition
            int before = reserveAmmo;
            reserveAmmo = Mathf.Min(reserveAmmo + wave.ammoRefillAmount, totalReserveAmmo);
            int actualAdded = reserveAmmo - before;

            if (actualAdded > 0)
                ShowRefillNotice($"+{actualAdded} AMMO");

            Debug.Log($"[AmmoSystem] Partial ammo refill +{actualAdded} applied for wave: {wave.waveName}");
        }
        // If both flags are false / zero — no ammo given this wave (intentional scarcity)

        RefreshUI();
        CheckDeathPanel(); // re-evaluate in case they were on 0 before the wave
    }

    // ─────────────────────────────────────────────
    //  Shooting
    // ─────────────────────────────────────────────

    public bool TryShoot()
    {
        if (isReloading || currentAmmo <= 0) return false;

        currentAmmo--;
        RefreshUI();

        if (currentAmmo == 0 && reserveAmmo == 0)
            TriggerNoAmmoDeath();

        return true;
    }

    // ─────────────────────────────────────────────
    //  Reloading
    // ─────────────────────────────────────────────

    /// <summary>Returns true if a reload is valid — mag not full AND reserve has ammo.</summary>
    public bool NeedsReload() => !isReloading && currentAmmo < magazineSize && reserveAmmo > 0;

    public void StartReload(Action onComplete = null)
    {
        if (isReloading) return;
        if (currentAmmo >= magazineSize || reserveAmmo <= 0) return;

        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = StartCoroutine(ReloadRoutine(onComplete));
    }

    private IEnumerator ReloadRoutine(Action onComplete)
    {
        isReloading = true;
        if (reloadingText != null) reloadingText.gameObject.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;
        isReloading = false;

        if (reloadingText != null) reloadingText.gameObject.SetActive(false);

        RefreshUI();
        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────────
    //  Death Panel
    // ─────────────────────────────────────────────

    private void TriggerNoAmmoDeath()
    {
        if (noAmmoDeathPanel != null)
            noAmmoDeathPanel.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("[AmmoSystem] No ammo remaining — death panel activated.");
    }

    /// <summary>Hides death panel if ammo is available again (e.g. after wave refill).</summary>
    private void CheckDeathPanel()
    {
        if (noAmmoDeathPanel == null) return;

        bool hasAmmo = currentAmmo > 0 || reserveAmmo > 0;
        if (hasAmmo && noAmmoDeathPanel.activeSelf)
        {
            noAmmoDeathPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // ─────────────────────────────────────────────
    //  UI Helpers
    // ─────────────────────────────────────────────

    private void RefreshUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }

    private void ShowRefillNotice(string message)
    {
        if (ammoRefillNoticeText == null) return;
        StopAllCoroutines(); // cancel any running notice fade
        StartCoroutine(RefillNoticeRoutine(message));
    }

    private IEnumerator RefillNoticeRoutine(string message)
    {
        ammoRefillNoticeText.text = message;
        ammoRefillNoticeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        ammoRefillNoticeText.gameObject.SetActive(false);
    }
}