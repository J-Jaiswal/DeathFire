using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioSource waveSource;
    [SerializeField] private AudioSource musicSource; // gameplay/completion music

    // ── Weapon SFX ────────────────────────────────────────────────────────────

    [Header("Weapon SFX")]
    [SerializeField] private AudioClip gunshotClip;
    [SerializeField] private AudioClip reloadStartClip;        // animation start
    [SerializeField] private AudioClip emptyGunFire;       // mag locked in
    [SerializeField] private AudioClip hoisterClip;
    [SerializeField] private AudioClip headshotClip;

    // ── Player SFX ────────────────────────────────────────────────────────────

    [Header("Player SFX")]
    [SerializeField] private AudioClip playerHurtClip;
    [SerializeField] private AudioClip playerDeathClip;
    [SerializeField] private AudioClip manScreamReloadClip;    

    // ── Wave / Game State SFX ─────────────────────────────────────────────────

    [Header("Wave SFX")]
    [SerializeField] private AudioClip waveStartClip;
    [SerializeField] private AudioClip waveClearClip;
    [SerializeField] private AudioClip countdownTickClip;      // per-tick beep
    //[SerializeField] private AudioClip wave1IntroClip;         // wave 1 special sting
    //[SerializeField] private AudioClip nextWaveCountdownClip;  // "next wave in X" audio
    [SerializeField] private AudioClip gameCompletionClip;     // all waves cleared music
    [SerializeField] private AudioClip firstWaveCount;
    // ── Enemy SFX ─────────────────────────────────────────────────────────────

    [Header("Enemy SFX")]
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip enemyDamage;            // vehicle/melee run-over

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    // ---------------- Events for wave ------------
    private void OnEnable()
    {
        //WaveEventBus.OnWaveStarted += HandleWaveStarted;
        WaveEventBus.OnWaveCleared += HandleWaveCleared;
        //WaveEventBus.OnCountdownTick += HandleCountdownTick;
        WaveEventBus.OnVictory += HandleVictory;
    }

    private void OnDisable()
    {
        //WaveEventBus.OnWaveStarted -= HandleWaveStarted;
        WaveEventBus.OnWaveCleared -= HandleWaveCleared;
        //WaveEventBus.OnCountdownTick -= HandleCountdownTick;
        WaveEventBus.OnVictory -= HandleVictory;
    }

    // ── Wave Event Handlers ───────────────────────────────────────────────────

    //private void HandleWaveStarted(int waveNumber)
    //{
    //    PlayWaveStart();
    //}

    private void HandleWaveCleared(int waveNumber)
    {
        PlayWaveClear();
    }

    //private void HandleCountdownTick(int remaining, int waveNumber)
    //{
    //    if (remaining == 0) return; // tick 0 is the "Get Ready!" beat — no beep needed

    //    if (remaining <= 3)         // only beep the last 3 seconds
    //        PlayCountdownTick();
    //}

    private void HandleVictory()
    {
        PlayGameCompletion();
    }

    // ── Weapon ────────────────────────────────────────────────────────────────

    public void PlayGunshot() => weaponSource.PlayOneShot(gunshotClip);
    public void PlayReload() {

        //weaponSource.PlayOneShot(manScreamReloadClip);
        weaponSource.PlayOneShot(reloadStartClip); }
    public void PlayEmptyGunFire() => weaponSource.PlayOneShot(emptyGunFire);
    public void PlayHoister() => weaponSource.PlayOneShot(hoisterClip);
    public void PlayHeadshot() => weaponSource.PlayOneShot(headshotClip);


    // ── Player ────────────────────────────────────────────────────────────────

    public void PlayPlayerHurt() => playerSource.PlayOneShot(playerHurtClip);
    public void PlayPlayerDeath() => playerSource.PlayOneShot(playerDeathClip);
    public void PlayEnemyDamage() => playerSource.PlayOneShot(enemyDamage);

    // ── Wave / Game State ─────────────────────────────────────────────────────

    public void PlayWaveStart() => waveSource.PlayOneShot(waveStartClip);
    public void PlayWaveClear() => waveSource.PlayOneShot(waveClearClip);
    public void PlayCountdownTick() => waveSource.PlayOneShot(countdownTickClip);
    //public void PlayWave1Intro() => waveSource.PlayOneShot(wave1IntroClip);
    public void FirstWaveCountdown() => waveSource.PlayOneShot(firstWaveCount);
    //public void PlayNextWaveCountdown() => waveSource.PlayOneShot(nextWaveCountdownClip);

    public void PlayGameCompletion()
    {
        if (musicSource == null || gameCompletionClip == null) return;
        musicSource.Stop();
        musicSource.loop = false;
        musicSource.PlayOneShot(gameCompletionClip);
    }

    // ── Enemy ─────────────────────────────────────────────────────────────────

    public void PlayEnemyDeath()
    {
        if (enemyDeathClip != null)
            weaponSource.PlayOneShot(enemyDeathClip);
    }

    // ── Utility ───────────────────────────────────────────────────────────────

    /// <summary>Play any clip at a world position (fire and forget).</summary>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void SetMasterVolume(float volume) => AudioListener.volume = Mathf.Clamp01(volume);
}