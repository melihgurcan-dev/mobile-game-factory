using UnityEngine;

/// <summary>
/// Simple audio manager for SFX and background music.
/// Uses the singleton pattern — keeps itself alive across scene loads.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips")]
    public AudioClip placeClip;    // Normal block placement
    public AudioClip perfectClip;  // Perfect placement
    public AudioClip failClip;     // Game over / miss

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume   = 0.8f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource   = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume   = sfxVolume;
        musicSource.volume = musicVolume;
        musicSource.loop   = true;
    }

    void Start()
    {
        if (backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayPlace()   => PlaySFX(placeClip);
    public void PlayPerfect() => PlaySFX(perfectClip != null ? perfectClip : placeClip);
    public void PlayFail()    => PlaySFX(failClip);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float v) { musicVolume = v; musicSource.volume = v; }
    public void SetSFXVolume(float v)   { sfxVolume   = v; sfxSource.volume   = v; }
    public void ToggleMusic(bool on)    { if (on) musicSource.Play(); else musicSource.Stop(); }
}
