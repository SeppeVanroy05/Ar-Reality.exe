using UnityEngine;

// Alleen effecten: de missie beslist zelf wanneer een tik juist of fout is.
public class MissionEffects : MonoBehaviour
{
    public static readonly Color Mint = new Color(0.22f, 1f, 0.78f);
    public static readonly Color Amber = new Color(1f, 0.72f, 0.2f);
    public static readonly Color Violet = new Color(0.64f, 0.32f, 1f);
    public GlitchMission mission;
    public Transform[] sparks;
    public Transform[] sceneryRings;
    private readonly Vector3[] velocities = new Vector3[18];
    private float burstTime;
    private AudioSource speaker;
    private AudioClip hitSound;
    private AudioClip errorSound;
    private AudioClip winSound;
    private static MaterialPropertyBlock colors;

    private void Awake()
    {
        speaker = GetComponent<AudioSource>();
        if (!Application.isPlaying || speaker == null) return;
        hitSound = MakeTone(660, 0.12f, false);
        errorSound = MakeTone(170, 0.12f, false);
        winSound = MakeTone(440, 0.5f, true);
    }

    public static void Tint(Renderer renderer, Color color)
    {
        colors ??= new MaterialPropertyBlock();
        renderer.GetPropertyBlock(colors);
        colors.SetColor("_Color", color);
        colors.SetColor("_EmissionColor", color * 0.65f);
        renderer.SetPropertyBlock(colors);
    }

    public void ResetEffects()
    {
        burstTime = 0;
        foreach (var spark in sparks) spark.gameObject.SetActive(false);
    }

    public void Hit(Vector3 position, bool celebration)
    {
        burstTime = 0.65f;
        for (int i = 0; i < sparks.Length; i++)
        {
            var spark = sparks[i];
            spark.gameObject.SetActive(true);
            spark.localPosition = position;
            spark.localScale = Vector3.one * (celebration ? 0.005f : 0.003f);
            float angle = i * Mathf.PI * 2 / sparks.Length;
            velocities[i] = new Vector3(Mathf.Cos(angle), 0.8f + (i % 3) * 0.3f, Mathf.Sin(angle)) * 0.07f;
            Tint(spark.GetComponent<Renderer>(), celebration ? Mint : i % 2 == 0 ? Mint : Amber);
        }
        if (Application.isPlaying && speaker != null && hitSound != null && !celebration)
        {
            speaker.pitch = 1;
            speaker.PlayOneShot(hitSound, 0.16f);
        }
    }

    public void Cue(int note)
    {
        if (!Application.isPlaying || speaker == null || hitSound == null) return;
        speaker.pitch = 0.85f + note * 0.25f;
        speaker.PlayOneShot(hitSound, 0.12f);
    }

    public void Reject()
    {
        if (Application.isPlaying && speaker != null && errorSound != null) speaker.PlayOneShot(errorSound, 0.1f);
    }

    public void Celebrate()
    {
        Hit(new Vector3(0, 0.06f, 0), true);
        if (Application.isPlaying && speaker != null && winSound != null) speaker.PlayOneShot(winSound, 0.18f);
    }

    private void Update()
    {
        if (!mission.IsPlaying || !mission.IsTracked) return;
        for (int i = 0; i < sceneryRings.Length; i++)
            sceneryRings[i].Rotate(Vector3.up, Time.deltaTime * (i % 2 == 0 ? 14 : -19), Space.Self);
        if (burstTime <= 0) return;
        burstTime -= Time.deltaTime;
        for (int i = 0; i < sparks.Length; i++)
        {
            sparks[i].localPosition += velocities[i] * Time.deltaTime;
            sparks[i].Rotate(new Vector3(90, 120, 45) * Time.deltaTime);
            if (burstTime <= 0) sparks[i].gameObject.SetActive(false);
        }
    }

    private static AudioClip MakeTone(float frequency, float duration, bool rising)
    {
        const int rate = 22050;
        float[] samples = new float[Mathf.CeilToInt(rate * duration)];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = i / (float)rate;
            float note = rising ? frequency * Mathf.Pow(2, Mathf.Floor(t * 8) / 3f) : frequency;
            samples[i] = Mathf.Sin(t * note * Mathf.PI * 2) * Mathf.Min(1, t * 100) * (1 - t / duration) * 0.35f;
        }
        var clip = AudioClip.Create("Reality feedback", samples.Length, 1, rate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private void OnDestroy()
    {
        if (hitSound != null) Destroy(hitSound);
        if (errorSound != null) Destroy(errorSound);
        if (winSound != null) Destroy(winSound);
    }
}
