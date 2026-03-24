using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class AmmoSystem : MonoBehaviour
{
    public static AmmoSystem Instance { get; private set; }

    [Header("Settings")]
    private readonly int magazineSize = 12;
    private readonly int totalReserveAmmo = 12;
    [SerializeField] private float reloadTime = 2.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private TextMeshProUGUI ammoRefillNoticeText; // optional — shows "+X Ammo" on refill


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
        RefreshUI();
    }

    
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

            //Debug.Log($"[AmmoSystem] Full ammo refill applied for wave: {wave.waveName}");
        }
        else if (wave.ammoRefillAmount > 0)
        {
            int totalPool = currentAmmo + reserveAmmo + wave.ammoRefillAmount;

            int previousTotal = currentAmmo + reserveAmmo;
            int actualAdded = totalPool - previousTotal; // = wave.ammoRefillAmount (for notice clarity)

            currentAmmo = Mathf.Min(totalPool, magazineSize);
            reserveAmmo = totalPool - currentAmmo;

            if (actualAdded > 0)
                ShowRefillNotice($"+{actualAdded} AMMO");

            Debug.Log($"[AmmoSystem] Wave refill: pool={totalPool} | " +
                      $"Magazine: {currentAmmo}/{magazineSize} | Reserve: {reserveAmmo} | " +
                      $"Wave: {wave.waveName}");
        }

        RefreshUI();
    }

    // ─────────────────────────────────────────────
    //  Shooting
    // ─────────────────────────────────────────────

    public bool TryShoot()
    {
        if (isReloading || currentAmmo <= 0) return false;

        currentAmmo--;
        RefreshUI();


        return true;
    }

    // ─────────────────────────────────────────────
    //  Reloading
    // ─────────────────────────────────────────────

   public bool CanReload() => !isReloading && currentAmmo < magazineSize && reserveAmmo > 0;

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
        //if (reloadingText != null) reloadingText.gameObject.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;
        isReloading = false;

        //if (reloadingText != null) reloadingText.gameObject.SetActive(false); 

        RefreshUI();
        onComplete?.Invoke();
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