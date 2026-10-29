using System.Collections;
using UnityEngine;
using TMPro;
using InfimaGames.LowPolyShooterPack; // for CharacterBehaviour

public class WeaponAnimationDriver : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
[Tooltip("The player character's Animator for fire‑empty and other character‑level states.")]
[SerializeField] private Animator characterAnimator;

    [Header("Fire Control")]
    [SerializeField] private float fireRate = 5f;

    [Header("Spawning")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private TextMeshProUGUI reloadingText;
    [SerializeField] private ParticleSystem muzzle;

    [Header("References")]
    [SerializeField] private AmmoSystem ammoSystem; // drag in via Inspector

    [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
    [SerializeField]
    private Transform socketEjection;

    [Header("Resources")]

    [Tooltip("Casing Prefab.")]
    [SerializeField]
    private GameObject prefabCasing;

    private float nextFireTime;
    private bool isReloading;
    private bool isInspecting;
    private bool isHolstered;

    private int layerOverlay = 0;
    private int layerActions = 0;
    private bool waveIsActive = true;
    private float reloadtimer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            int idxOverlay = animator.GetLayerIndex("Layer Overlay");
            layerOverlay = idxOverlay >= 0 ? idxOverlay : 0;

            int idxActions = animator.GetLayerIndex("Layer Actions");
            layerActions = idxActions >= 0 ? idxActions : 0;
        }

        // Find the player character's animator for fire‑empty handling.
        if (characterAnimator == null)
        {
            var characterBehaviour = FindObjectOfType<CharacterBehaviour>();
            if (characterBehaviour != null)
                characterAnimator = characterBehaviour.GetComponent<Animator>();
        }

        if (reloadingText != null) reloadingText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (WaveManager.Instance != null && !WaveManager.Instance.PlayerAlive) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
            TryFire();

        if (Input.GetKeyDown(KeyCode.R))
            TryReload();

        if (Input.GetKeyDown(KeyCode.T))
            TryInspect();

        if (Input.GetKeyDown(KeyCode.H))
            TryToggleHolster();
    }

    // Firing ----------------->

    public void TryFire()
    {
        if (WaveSpawnController.Instance != null)
            waveIsActive = WaveSpawnController.Instance.WaveInProgress;
        else
            waveIsActive = true;

        if (isReloading || isInspecting || isHolstered) return;
        if (Time.time < nextFireTime) return;
        if (!waveIsActive) return;

        nextFireTime = Time.time + (1f / fireRate);

        if (ammoSystem != null && ammoSystem.IsEmpty)
        {
            if (SFXManager.Instance != null) SFXManager.Instance.PlayEmptyGunFire();
            // Fire‑empty is a character‑level animation in the Infima pack.
            if (characterAnimator != null)
                characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0f);
            else
                PlayAnimation("Fire Empty", layerOverlay);
            return;
        }

        PlayAnimation("Fire", layerOverlay);
        StartCoroutine(ReturnToIdleAfter(0.2f));

        if (SFXManager.Instance != null) SFXManager.Instance.PlayGunshot();
        if (WaveStatsTracker.Instance != null) WaveStatsTracker.Instance.RegisterFired();
        if (ammoSystem != null) ammoSystem.TryShoot();

        SpawnBullet();
        PlayMuzzleFlash();
    }

    // Reload ------------------------->

    public void TryReload()
    {
        if (isReloading || isInspecting || isHolstered) return;
        if (ammoSystem != null && !ammoSystem.CanReload()) return;

        if (WaveSpawnController.Instance != null)
            waveIsActive = WaveSpawnController.Instance.WaveInProgress;
        else
            waveIsActive = true;

        if (!waveIsActive) return;

        isReloading = true;
        if (SFXManager.Instance != null) SFXManager.Instance.PlayReload();

        string animName = (ammoSystem != null && ammoSystem.IsEmpty) ? "Reload Empty" : "Reload";
        PlayAnimation(animName, layerActions);

        if (reloadingText != null)
            StartCoroutine(ReloadingTimer(2.5f));

        if (ammoSystem != null)
            ammoSystem.StartReload(OnReloadComplete);
        else
            StartCoroutine(FallbackReloadRoutine(2.5f));
    }

    private IEnumerator FallbackReloadRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnReloadComplete();
    }

    private void OnReloadComplete()
    {
        isReloading = false;
        // Return to idle after reload completes.
        StartCoroutine(ReturnToIdleAfter(0.1f));
    }

    // Holster / Unholster ------------------->

    public void TryToggleHolster()
    {
        if (isReloading || isInspecting) return;
        if (isHolstered)
            StartCoroutine(UnholsterRoutine());
        else
            StartCoroutine(HolsterRoutine());
    }

    private IEnumerator HolsterRoutine()
    {
        isHolstered = true;
        if (SFXManager.Instance != null) SFXManager.Instance.PlayHoister();
        PlayAnimation("Holster", layerActions);
        yield return new WaitForSeconds(1.2f);
    }

    private IEnumerator UnholsterRoutine()
    {
        if (SFXManager.Instance != null) SFXManager.Instance.PlayHoister();
        PlayAnimation("Unholster", layerActions);
        yield return new WaitForSeconds(1.2f);
        isHolstered = false;
    }

    // Inspect ---------------------->

    public void TryInspect()
    {
        if (isReloading || isInspecting || isHolstered) return;
        StartCoroutine(InspectRoutine());
    }

    private IEnumerator InspectRoutine()
    {
        isInspecting = true;
        if (SFXManager.Instance != null) SFXManager.Instance.PlayHoister();
        PlayAnimation("Inspect", layerActions);
        yield return new WaitForSeconds(4.8f);
        // Return to idle after inspect finishes.
        PlayAnimation("Idle", 0);
        isInspecting = false;
    }

    // Helper ------------------------>

    private void PlayAnimation(string stateName, int layer = 0)
    {
        if (animator == null) return;
        animator.CrossFade(stateName, 0.05f, layer, 0f);
    }

    // Helper coroutine to return to idle after a short delay.
    private IEnumerator ReturnToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAnimation("Idle", 0);
    }

    // Gameplay Hooks --------------------->

    private IEnumerator ReloadingTimer(float duration)
    {
        float elapsed = 0f;
        if (reloadingText != null) reloadingText.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            reloadtimer = duration - elapsed;
            if (reloadingText != null) reloadingText.text = $"R ({reloadtimer:F1})";
            elapsed += Time.deltaTime;
            yield return null;
        }

        reloadtimer = 0f;
        if (reloadingText != null)
        {
            reloadingText.text = $"R (0.0)";
            yield return new WaitForSeconds(0.15f);
            reloadingText.gameObject.SetActive(false);
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null && muzzlePoint != null)
            Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
    }

    private void PlayMuzzleFlash()
    {
        if (muzzle != null) muzzle.Play();
    }

    // Infima Animation Event Receivers ----------------------->
    // Dummy receivers so Unity does not output missing method warnings when playing Infima animations.
  
    private void OnEjectCasing()
    {
        //Spawn casing prefab at spawn point.
        if (prefabCasing != null && socketEjection != null)
            Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);
    }
    private void OnAmmunitionFill(int amount = 0) { }
    private void OnSetActiveMagazine(int active) { }
    private void OnSetActiveKnife(int active) { }
    private void OnGrenade() { }
    private void OnAnimationEndedBolt() { }
    private void OnAnimationEndedReload() { }
    private void OnAnimationEndedGrenadeThrow() { }
    private void OnAnimationEndedMelee() { }
    private void OnAnimationEndedInspect() { }
    private void OnAnimationEndedHolster() { }
    private void OnSlideBack(int back = 0) { }
}