using UnityEngine;


public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource musicSource; // looping background music

    [Header("Button SFX")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip buttonHoverClip;

    [Header("Background Music")]
    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    // ── Buttons ───────────────────────────────────────────────────────────────

    public void PlayButtonClick() => uiSource.PlayOneShot(buttonClickClip);
    public void PlayButtonHover() => uiSource.PlayOneShot(buttonHoverClip);

    // ── Music ─────────────────────────────────────────────────────────────────

    public void PlayMenuMusic()
    {
        if (menuMusicClip == null || musicSource == null) return;
        musicSource.clip = menuMusicClip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetMasterVolume(float volume) => AudioListener.volume = Mathf.Clamp01(volume);
}