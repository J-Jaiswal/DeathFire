using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponAnimationDriver : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Fire Control")]
    [SerializeField] private float fireRate = 5f;

    [Header("Spawning")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private TextMeshProUGUI reloadingText;
    [SerializeField] private ParticleSystem muzzle;


    [Header("References")]
    [SerializeField] private AmmoSystem ammoSystem; // drag in via Inspector

    private float nextFireTime;
    private bool isReloading;
    private bool isInspecting;

    private int layerOverlay;
    private int layerActions;
    bool waveIsActive;
    private float reloadtimer;
    
  

    private void Awake()
    {
        layerOverlay = animator.GetLayerIndex("Layer Overlay");
        layerActions = animator.GetLayerIndex("Layer Actions");
        if (reloadingText != null) reloadingText.gameObject.SetActive(false);

        WaveStatsTracker waveStatsTracker = WaveStatsTracker.Instance;
    }

    private void Update()
    {
        if ( !WaveManager.Instance.PlayerAlive) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
            TryFire();

        if (Input.GetKeyDown(KeyCode.R))
            TryReload();

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(StartInspect());
    }

    // ── Firing ────────────────────────────────────────────────────────────────

    public void TryFire()
    {
        waveIsActive = WaveSpawnController.Instance.WaveInProgress;

        if (isReloading || isInspecting ) return;

        if (Time.time < nextFireTime) return;
        if (!waveIsActive) return;

        
        nextFireTime = Time.time + (1f / fireRate);

        if(ammoSystem.IsEmpty)
        {
            SFXManager.Instance.PlayEmptyGunFire();
            animator.CrossFade("Fire Empty", 0.03f, layerOverlay, 0f);

            return;
        }
        
        animator.CrossFade("Fire", 0.03f, layerOverlay, 0f);
        SFXManager.Instance.PlayGunshot();
        WaveStatsTracker.Instance.RegisterFired();
        ammoSystem.TryShoot();
        SpawnBullet();
        PlayMuzzleFlash();
    }

    // ── Reload ────────────────────────────────────────────────────────────────

    public void TryReload()
    {
        // AmmoSystem decides if reload is actually needed
        if (isReloading || isInspecting) return;
        if (!ammoSystem.CanReload()) return;
        if (!waveIsActive) return;  
        isReloading = true;
        SFXManager.Instance.PlayReload();

        // Pick animation: full mag or empty mag
        string animName = ammoSystem.IsEmpty ? "Reload Empty" : "Reload";
        animator.Play(animName, layerActions, 0f);
        StartCoroutine(ReloadingTimer(2.5f));


        // Tell AmmoSystem reload started — it shows the UI progress bar
        // Pass our OnReloadComplete so AmmoSystem calls us back when done
        ammoSystem.StartReload(OnReloadComplete);
    }

    // Called back by AmmoSystem when the reload timer finishes
    private void OnReloadComplete()
    {
        isReloading = false;
        // AmmoSystem already refilled ammo and updated UI by this point
    }

    // ── Inspect ───────────────────────────────────────────────────────────────

    public IEnumerator StartInspect()
    {
        if (isReloading || isInspecting || !waveIsActive)
            
        yield break;
        isInspecting = true;
        SFXManager.Instance.PlayHoister();
        animator.Play("Inspect", layerActions, 0f);
        yield return new WaitForSeconds(4.8f);
        isInspecting = false;   
    }

    // ── Gameplay Hooks ────────────────────────────────────────────────────────
    
    private IEnumerator ReloadingTimer(float duration)
    {
        float elapsed = 0f;
        reloadingText.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            reloadtimer = duration - elapsed;
            reloadingText.text = $"R ({reloadtimer:F1})";
            elapsed += Time.deltaTime;
            yield return null;
        }

        reloadtimer = 0f;
        reloadingText.text = $"R (0.0)";
        yield return new WaitForSeconds(0.15f);
        reloadingText.gameObject.SetActive(false);
    }
    private void SpawnBullet()
    {
        if (bulletPrefab != null && muzzlePoint != null)
            Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
    }

    private void PlayMuzzleFlash()
    {
        muzzle.Play();
    }
}