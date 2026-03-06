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
    [Range(0f, 1f)] public float musicVolume = 0.55f;
    [Range(0f, 1f)] public float sfxVolume   = 0.85f;

    // Generated fallback clips
    private AudioClip _genPlace;
    private AudioClip _genPerfect;
    private AudioClip _genCombo;
    private AudioClip _genTap;
    private AudioClip _genFail;
    private AudioClip _genMusic;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        sfxSource   = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume        = sfxVolume;
        sfxSource.spatialBlend  = 0f;   // 2D — no distance falloff
        sfxSource.playOnAwake   = false;
        musicSource.volume      = musicVolume;
        musicSource.spatialBlend = 0f;
        musicSource.loop        = true;

        // Generate procedural clips
        try { _genPlace   = ProceduralAudio.Place();   } catch (System.Exception e) { Debug.LogError($"[AM] Place failed: {e}"); }
        try { _genPerfect = ProceduralAudio.Perfect(); } catch (System.Exception e) { Debug.LogError($"[AM] Perfect failed: {e}"); }
        try { _genCombo   = ProceduralAudio.Combo();   } catch (System.Exception e) { Debug.LogError($"[AM] Combo failed: {e}"); }
        try { _genTap     = ProceduralAudio.Tap();     } catch (System.Exception e) { Debug.LogError($"[AM] Tap failed: {e}"); }
        try { _genFail    = ProceduralAudio.Fail();    } catch (System.Exception e) { Debug.LogError($"[AM] Fail failed: {e}"); }
        try { _genMusic   = ProceduralAudio.Music();   } catch (System.Exception e) { Debug.LogError($"[AM] Music failed: {e}"); }
        Debug.Log($"[AM] Clips — place={_genPlace}, perfect={_genPerfect}, combo={_genCombo}, tap={_genTap}, fail={_genFail}, music={_genMusic}");
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        // Use Inspector override if assigned, else procedural (use Unity == for fake-null check)
        AudioClip music = backgroundMusic == null ? _genMusic : backgroundMusic;
        if (music != null)
        {
            musicSource.clip   = music;
            musicSource.loop   = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

    }

    public void PlayPlace()   => PlaySFX(placeClip   != null ? placeClip   : _genPlace);
    public void PlayPerfect() => PlaySFX(perfectClip != null ? perfectClip : _genPerfect);
    public void PlayCombo()   => PlaySFX(comboClip   != null ? comboClip   : _genCombo);
    public void PlayTap()     => PlaySFX(tapClip     != null ? tapClip     : _genTap);
    public void PlayFail()    => PlaySFX(failClip    != null ? failClip    : _genFail);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("[AudioManager] clip is null"); return; }
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float v) { musicVolume = v; if (musicSource) musicSource.volume = v; }
    public void SetSFXVolume(float v)   { sfxVolume   = v; if (sfxSource)   sfxSource.volume   = v; }
    public void ToggleMusic(bool on)    { if (!musicSource) return; if (on) musicSource.Play(); else musicSource.Stop(); }
}
