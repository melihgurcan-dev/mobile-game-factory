using UnityEngine;

/// <summary>
/// Generates AudioClips entirely from math — no audio files needed.
/// Works on WebGL, Android, and Editor.
/// </summary>
public static class ProceduralAudio
{
    private const int SAMPLE_RATE = 44100;

    // ── Public clip factories ──────────────────────────────────────────────

    /// Short click when placing a block
    public static AudioClip Tap() =>
        GenerateTone(new[]{ (600f, 1f) }, 0.06f, 0.35f, fadeOut: true);

    /// Chime when block is placed (non-perfect)
    public static AudioClip Place()
    {
        // Two-tone pluck: root + fifth
        return GenerateArpeggio(new[] { 440f, 660f }, 0.07f, 0.3f, 0.25f);
    }

    /// Ascending C-E-G arpeggio for perfect placement
    public static AudioClip Perfect()
    {
        return GenerateArpeggio(new[] { 523f, 659f, 784f }, 0.09f, 0.28f, 0.45f);
    }

    /// Faster, louder arpeggio for combo streaks
    public static AudioClip Combo()
    {
        return GenerateArpeggio(new[] { 523f, 659f, 784f, 1047f }, 0.07f, 0.22f, 0.6f);
    }

    /// Descending thud for game over
    public static AudioClip Fail()
    {
        int samples = (int)(SAMPLE_RATE * 0.5f);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq = Mathf.Lerp(280f, 80f, t / 0.5f);
            float env  = 1f - (t / 0.5f);
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * env * 0.55f;
            // add a bit of noise for "thud" character
            data[i] += Random.Range(-0.08f, 0.08f) * env;
        }
        return ToClip("Fail", data);
    }

    // ── Internals ──────────────────────────────────────────────────────────

    private static AudioClip GenerateTone(
        (float freq, float amp)[] tones,
        float duration, float volume, bool fadeOut)
    {
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];
        foreach (var (freq, amp) in tones)
        {
            for (int i = 0; i < samples; i++)
            {
                float t   = (float)i / SAMPLE_RATE;
                float env = fadeOut ? 1f - (t / duration) : 1f;
                data[i] += Mathf.Sin(2 * Mathf.PI * freq * t) * amp * env * volume;
            }
        }
        return ToClip("Tone", data);
    }

    private static AudioClip GenerateArpeggio(
        float[] notes, float noteDuration, float overlap, float volume)
    {
        float totalDuration = noteDuration * notes.Length + overlap;
        int   totalSamples  = (int)(SAMPLE_RATE * totalDuration);
        float[] data        = new float[totalSamples];

        for (int n = 0; n < notes.Length; n++)
        {
            float startTime = n * noteDuration;
            int   startSample = (int)(startTime * SAMPLE_RATE);
            int   noteSamples = (int)((noteDuration + overlap) * SAMPLE_RATE);

            for (int i = 0; i < noteSamples && startSample + i < totalSamples; i++)
            {
                float t   = (float)i / SAMPLE_RATE;
                float env = 1f - (t / (noteDuration + overlap));
                data[startSample + i] +=
                    Mathf.Sin(2 * Mathf.PI * notes[n] * t) * env * volume;
            }
        }
        return ToClip("Arpeggio", data);
    }

    private static AudioClip ToClip(string name, float[] data)
    {
        var clip = AudioClip.Create(name, data.Length, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }
}
