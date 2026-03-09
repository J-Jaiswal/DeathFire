using System.Collections;
using UnityEngine;

public class WeaponAnimationDriver : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Fire Control")]
    [SerializeField] private float fireRate = 5f;

    [Header("Spawning")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;

    [Header("References")]
    [SerializeField] private AmmoSystem ammoSystem; // drag in via Inspector

    private float nextFireTime;
    private bool isReloading;
    private bool isInspecting;
    private bool isHolstered;

    private int layerOverlay;
    private int layerActions;
    bool waveIsActive;
  

    private void Awake()
    {
        layerOverlay = animator.GetLayerIndex("Layer Overlay");
        layerActions = animator.GetLayerIndex("Layer Actions");

        WaveStatsTracker waveStatsTracker = WaveStatsTracker.Instance;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryFire();

        if (Input.GetKeyDown(KeyCode.R))
            TryReload();

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine (StartInspect());
    }

    // ── Firing ────────────────────────────────────────────────────────────────

    public void TryFire()
    {
        waveIsActive = WaveSpawnController.Instance.WaveInProgress;

        if (isReloading || isInspecting || isHolstered) return;

        if (Time.time < nextFireTime) return;
        if (!waveIsActive) return;

        // Ask AmmoSystem for permission — it decrements and handles auto-reload trigger
        if (!ammoSystem.TryShoot())
        {
            // Out of ammo: trigger reload automatically
            TryReload();
            return;
        }
        //WaveStatsTracker.Instance.RegisterShot(false);
        nextFireTime = Time.time + (1f / fireRate);
        //WaveStatsTracker.Instance?.RegisterShot(hit: didHit);
        animator.CrossFade("Fire", 0.03f, layerOverlay, 0f);
        WaveStatsTracker.Instance.RegisterFired();
        SpawnBullet();
        PlayMuzzleFlash();
    }

    // ── Reload ────────────────────────────────────────────────────────────────

    public void TryReload()
    {
        // AmmoSystem decides if reload is actually needed
        if (isReloading || isInspecting) return;
        if (!ammoSystem.NeedsReload()) return;
        isReloading = true;

        // Pick animation: full mag or empty mag
        string animName = ammoSystem.IsEmpty ? "Reload Empty" : "Reload";
        animator.Play(animName, layerActions, 0f);


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
        if (isReloading || isInspecting || isHolstered || !waveIsActive)
            
        yield break;
        isInspecting = true;
        animator.Play("Inspect", layerActions, 0f);
        yield return new WaitForSeconds(2f);
        isInspecting = false;   
    }

    // ── Gameplay Hooks ────────────────────────────────────────────────────────

    private void SpawnBullet()
    {
        if (bulletPrefab != null && muzzlePoint != null)
            Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
    }

    private void PlayMuzzleFlash()
    {
        // ParticleSystem.Play() here
    }
}