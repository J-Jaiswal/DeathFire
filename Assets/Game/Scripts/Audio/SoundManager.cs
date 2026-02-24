using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources — add these AudioSource components to this GameObject")]
    [SerializeField] private AudioSource weaponSource;   // for gunshots, reloads
    [SerializeField] private AudioSource uiSource;       // for UI, wave banners
    [SerializeField] private AudioSource playerSource;   // for hurt, death
    [SerializeField] private AudioSource ambientSource;  // looping background

    [Header("Weapon SFX")]
    [SerializeField] private AudioClip gunshotClip;
    [SerializeField] private AudioClip reloadStartClip;
    [SerializeField] private AudioClip reloadFinishClip;
    [SerializeField] private AudioClip emptyMagClickClip;
    [SerializeField] private AudioClip headshotClip;

    [Header("Player SFX")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Wave SFX")]
    [SerializeField] private AudioClip waveStartClip;
    [SerializeField] private AudioClip waveClearClip;
    [SerializeField] private AudioClip countdownTickClip;
    [SerializeField] private AudioClip allWavesClearClip;

    [Header("Enemy SFX")]
    [SerializeField] private AudioClip enemyDeathClip;

    private void Awake()
    {
        // Simple singleton — one AudioManager lives forever
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Weapon ────────────────────────────────────────────────────────────────

    public void PlayGunshot() => weaponSource.PlayOneShot(gunshotClip);
    public void PlayReloadStart() => weaponSource.PlayOneShot(reloadStartClip);
    public void PlayReloadFinish() => weaponSource.PlayOneShot(reloadFinishClip);
    public void PlayEmptyMagClick() => weaponSource.PlayOneShot(emptyMagClickClip);
    public void PlayHeadshot() => weaponSource.PlayOneShot(headshotClip);

    // ── Player ────────────────────────────────────────────────────────────────

    public void PlayHurt() => playerSource.PlayOneShot(hurtClip);
    public void PlayDeath() => playerSource.PlayOneShot(deathClip);

    // ── Wave ──────────────────────────────────────────────────────────────────

    public void PlayWaveStart() => uiSource.PlayOneShot(waveStartClip);
    public void PlayWaveClear() => uiSource.PlayOneShot(waveClearClip);
    public void PlayCountdownTick() => uiSource.PlayOneShot(countdownTickClip);
    public void PlayAllWavesClear() => uiSource.PlayOneShot(allWavesClearClip);

    // ── Enemy ─────────────────────────────────────────────────────────────────

    public void PlayEnemyDeath()
    {
        if (enemyDeathClip != null)
            weaponSource.PlayOneShot(enemyDeathClip);
    }

    // ── Utility ───────────────────────────────────────────────────────────────

    /// <summary>Play any clip at a world position (fire and forget, no source needed).</summary>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void SetMasterVolume(float volume) => AudioListener.volume = volume;
}