using System.Collections;
using UnityEngine;

public class WeaponAnimationDriver : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Fire Control")]
    [SerializeField] private float fireRate = 5f; // shots per second

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform muzzlePoint;
    //[SerializeField] AudioClip gunshotClip; 
    //[SerializeField] AudioSource gunAudioSource;
    //[SerializeField] AudioClip fireClip;

    private float nextFireTime;

    private bool isReloading;
    private bool isInspecting;
    private bool isHolstered;

    private int layerOverlay;
    private int layerActions;

    private static readonly int HashAiming = Animator.StringToHash("Aiming");

    private void Awake()
    {
        layerOverlay = animator.GetLayerIndex("Layer Overlay");
        layerActions = animator.GetLayerIndex("Layer Actions");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();

        if (Input.GetKeyDown(KeyCode.R))
            Reload(true);

        if (Input.GetKeyDown(KeyCode.T))
            StartInspect();

        //SetAiming(Input.GetMouseButton(1));
    }

    /* ---------------- FIRING ---------------- */

    public void Fire()
    {
        if (isReloading || isInspecting || isHolstered)
            return;

       
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + (1f / fireRate);

        
        animator.CrossFade("Fire", 0.03f, layerOverlay, 0f);

        // 🎯 Gameplay happens HERE (not in animation)
        SpawnBullet();
        PlayMuzzleFlash();
        //PlayGunshotSound();
    }

    /* ---------------- GAMEPLAY HOOKS ---------------- */

    public void StartInspect()
    {
        if (isReloading || isInspecting || isHolstered)
            return;

        animator.Play("Inspect", layerActions, 0f);

    }

  

    private void SpawnBullet()
    {
        Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
        
        // Instantiate projectile from muzzle
    }

    private void PlayMuzzleFlash()
    {
        
        // ParticleSystem.Play()
    }

    //private void PlayGunshotSound()
    //{
    //    if (gunAudioSource == null || fireClip == null)
    //    {
    //        Debug.LogError("AudioSource or FireClip missing!");
    //        return;
    //    }
    //    Debug.Log("Gunshot sound triggered");
    //    gunAudioSource.PlayOneShot(fireClip);

       
    //}

    /* ---------------- RELOAD ---------------- */

    public void Reload(bool hasAmmo)
    {
        if (isReloading || isInspecting)
            return;

        animator.Play(hasAmmo ? "Reload" : "Reload Empty", layerActions, 0f);
        isReloading = true;

        StartCoroutine(ReloadFinished(2f)); 
    }

    private IEnumerator ReloadFinished( float duration)
    {
        yield return new WaitForSeconds(duration);
        isReloading = false;
    }   

    
  
   

    /* ---------------- AIM ---------------- */

    //public void SetAiming(bool value)
    //{
    //    if (isReloading || isInspecting || isHolstered)
    //        return;

    //    animator.SetBool("Aim", value);
    //    animator.SetFloat(HashAiming, value ? 1f : 0f);
    //}
}
