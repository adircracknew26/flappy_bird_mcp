using UnityEngine;

/// <summary>Efectos de sonido sintetizados en tiempo de ejecución (sin archivos externos).</summary>
public class Sfx : MonoBehaviour
{
    private static Sfx instance;

    private AudioSource source;
    private AudioClip flapClip;
    private AudioClip scoreClip;
    private AudioClip hitClip;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.35f;

        flapClip = CreateSweep("Flap", 0.07f, 420f, 760f, 0.5f);
        scoreClip = CreateSweep("Score", 0.10f, 880f, 1320f, 0.45f);
        hitClip = CreateHit("Hit", 0.20f, 0.6f);
    }

    public static void PlayFlap()
    {
        if (instance != null) instance.source.PlayOneShot(instance.flapClip);
    }

    public static void PlayScore()
    {
        if (instance != null) instance.source.PlayOneShot(instance.scoreClip);
    }

    public static void PlayHit()
    {
        if (instance != null) instance.source.PlayOneShot(instance.hitClip);
    }

    private static AudioClip CreateSweep(string name, float duration, float startHz, float endHz, float volume)
    {
        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(duration * sampleRate);
        float[] data = new float[samples];
        float phase = 0f;

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)samples;
            float frequency = Mathf.Lerp(startHz, endHz, t);
            phase += 2f * Mathf.PI * frequency / sampleRate;
            float envelope = Mathf.Sin(Mathf.PI * t);
            data[i] = Mathf.Sin(phase) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip CreateHit(string name, float duration, float volume)
    {
        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)samples;
            float envelope = (1f - t) * (1f - t);
            data[i] = Random.Range(-1f, 1f) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
