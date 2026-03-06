using UnityEngine;

/// <summary>
/// Generates AudioClips entirely from math — no audio files needed.
/// </summary>
public static class ProceduralAudio
{
    private const int SR = 44100;

    public static AudioClip Tap()     => Tone(800f, 0.05f, 0.7f);
    public static AudioClip Place()   => TwoTone(440f, 550f, 0.08f, 0.5f);
    public static AudioClip Perfect() => TwoTone(523f, 784f, 0.12f, 0.7f);
    public static AudioClip Combo()   => TwoTone(659f, 1047f, 0.12f, 0.8f);
    public static AudioClip Fail()    => Sweep(300f, 80f, 0.4f, 0.5f);

    /// Upbeat retro space arcade — C-major pentatonic chip-tune arpeggio, 9.6 s seamless loop (6 phrases)
    public static AudioClip Music()
    {
        // C-major pentatonic notes (Hz)
        // Phrase 1: Gentle rise & fall
        // Phrase 2: Bigger peak (E6 = 1318.5)
        // Phrase 3: Lower groove (G4 = 392)
        // Phrase 4: Triumphant climb
        // Phrase 5: Rhythmic bounce
        // Phrase 6: Big finale
        float[] melody = {
            523.25f, 659.25f, 784f,    880f,    1046.5f, 880f,    784f,    659.25f,  // ph1
            523.25f, 659.25f, 880f,    1046.5f, 1318.5f, 1046.5f, 880f,    784f,     // ph2
            392f,    523.25f, 659.25f, 784f,    659.25f, 523.25f, 659.25f, 784f,     // ph3
            523.25f, 784f,    1046.5f, 1318.5f, 1046.5f, 784f,   659.25f, 523.25f,   // ph4
            659.25f, 784f,   659.25f,  880f,    784f,    659.25f, 523.25f, 784f,     // ph5
            523.25f, 659.25f, 784f,    880f,    1046.5f, 1318.5f, 880f,   523.25f,   // ph6
        };
        float[] bassNotes = {
            130.81f, 196f,    174.61f, 196f,
            130.81f, 196f,    174.61f, 130.81f,
            98f,     130.81f, 174.61f, 196f,
            130.81f, 196f,    130.81f, 196f,
            130.81f, 174.61f, 196f,    130.81f,
            130.81f, 196f,    174.61f, 130.81f,
        };

        float noteDur  = 0.20f;   // 150 BPM eighth notes
        float overlap  = 0.07f;
        float totalDur = melody.Length * noteDur + overlap;
        int   total    = Mathf.Max(1, (int)(SR * totalDur));
        var   d        = new float[total];

        // Melody: chip-tone sine + octave harmonic
        for (int ni = 0; ni < melody.Length; ni++)
        {
            float freq  = melody[ni];
            int   start = (int)(ni * noteDur * SR);
            int   len   = Mathf.Min((int)((noteDur + overlap) * SR), total - start);
            for (int i = 0; i < len; i++)
            {
                float t   = (float)i / SR;
                float env = Mathf.Clamp01(1f - t / (noteDur + overlap));
                env = env * env;  // quick decay = snappy chip feel
                float s   = Mathf.Sin(2f * Mathf.PI * freq * t)        * 0.38f
                          + Mathf.Sin(2f * Mathf.PI * freq * 2f * t)   * 0.13f
                          + Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t) * 0.07f;
                if (start + i < total) d[start + i] += s * env;
            }
        }

        // Bass: one note per 2 melody notes
        float bassDur = noteDur * 2f;
        for (int bi = 0; bi < bassNotes.Length; bi++)
        {
            int start = (int)(bi * bassDur * SR);
            int len   = Mathf.Min((int)(bassDur * 0.9f * SR), total - start);
            for (int i = 0; i < len; i++)
            {
                float t   = (float)i / SR;
                float env = 1f - t / (bassDur * 0.9f);
                if (start + i < total)
                    d[start + i] += Mathf.Sin(2f * Mathf.PI * bassNotes[bi] * t) * env * 0.30f;
            }
        }

        // Seamless loop: fade out last 0.30 s
        int fadeLen = (int)(0.30f * SR);
        for (int i = 0; i < fadeLen && i < total; i++)
            d[total - 1 - i] *= (float)i / fadeLen;

        return Make("ArcadeSpace", d);
    }

    // ── Primitives ─────────────────────────────────────────────────────────

    static AudioClip Tone(float freq, float dur, float vol)
    {
        int n = Mathf.Max(1, (int)(SR * dur));
        var d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t   = (float)i / SR;
            float env = 1f - (t / dur);
            d[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * vol;
        }
        return Make("Tone", d);
    }

    static AudioClip TwoTone(float f1, float f2, float dur, float vol)
    {
        int n = Mathf.Max(1, (int)(SR * dur));
        var d = new float[n];
        int half = n / 2;
        for (int i = 0; i < n; i++)
        {
            float freq = i < half ? f1 : f2;
            float t    = (float)i / SR;
            float env  = 1f - ((float)i / n);
            d[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * vol;
        }
        return Make("TwoTone", d);
    }

    static AudioClip Sweep(float f0, float f1, float dur, float vol)
    {
        int n = Mathf.Max(1, (int)(SR * dur));
        var d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t    = (float)i / SR;
            float freq = Mathf.Lerp(f0, f1, (float)i / n);
            float env  = 1f - (t / dur);
            d[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * vol;
        }
        return Make("Sweep", d);
    }

    static AudioClip Make(string name, float[] data)
    {
        Debug.Log($"[PA] Make called: {name}, samples={data.Length}");
        var c = AudioClip.Create(name, data.Length, 1, SR, false);
        if (c == null) { Debug.LogError($"[PA] AudioClip.Create returned null for {name}"); return null; }
        c.SetData(data, 0);
        Debug.Log($"[PA] Make OK: {name}");
        return c;
    }
}
