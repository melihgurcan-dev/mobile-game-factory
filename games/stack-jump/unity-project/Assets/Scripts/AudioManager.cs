using UnityEngine;

/// <summary>
/// Audio manager with procedural fallback — no audio files required.
/// Assign optional AudioClip overrides in the Inspector to replace generated sounds.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips (optional — procedural fallback if empty)")]
    public AudioClip placeClip;    // Normal block placement
    public AudioClip perfectClip;  // Perfect placement
    public AudioClip comboClip;    // Combo streak
    public AudioClip tapClip;      // UI tap / menu tap
    public AudioClip failClip;     // Game over

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume   = 0.85f;

    // Generated fallback clips
    private AudioClip _genPlace;
    private AudioClip _genPerfect;
    private AudioClip _genCombo;
    private AudioClip _genTap;
    private AudioClip _genFail;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        sfxSource   = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume   = sfxVolume;
        musicSource.volume = musicVolume;
        musicSource.loop   = true;

        // Generate procedural clips on background thread equivalent (instant, PCM math)
        _genPlace   = ProceduralAudio.Place();
        _genPerfect = ProceduralAudio.Perfect();
        _genCombo   = ProceduralAudio.Combo();
        _genTap     = ProceduralAudio.Tap();
        _genFail    = ProceduralAudio.Fail();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayPlace()   => PlaySFX(placeClip   ?? _genPlace);
    public void PlayPerfect() => PlaySFX(perfectClip ?? _genPerfect);
    public void PlayCombo()   => PlaySFX(comboClip   ?? _genCombo);
    public void PlayTap()     => PlaySFX(tapClip     ?? _genTap);
    public void PlayFail()    => PlaySFX(failClip    ?? _genFail);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float v) { musicVolume = v; if (musicSource) musicSource.volume = v; }
    public void SetSFXVolume(float v)   { sfxVolume   = v; if (sfxSource)   sfxSource.volume   = v; }
    public void ToggleMusic(bool on)    { if (!musicSource) return; if (on) musicSource.Play(); else musicSource.Stop(); }
}
