using UnityEngine;
using System.Collections;

/// <summary>
/// Visual juice: screen shake, particle bursts, and block squish.
/// </summary>
public class JuiceEffects : MonoBehaviour
{
    public static JuiceEffects Instance { get; private set; }

    [Header("Particles")]
    public ParticleSystem milestoneParticles;
    public ParticleSystem perfectParticles;

    [Header("Screen Shake")]
    public Camera targetCamera;
    [Range(0f, 1f)] public float shakeDuration  = 0.25f;
    [Range(0f, 1f)] public float shakeMagnitude = 0.15f;

    private Vector3 originalCamLocalPos;
    private bool    isShaking;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnEnable()
    {
        GameManager.OnGameOver    += TriggerShake;
        ScoreManager.OnScoreChanged += HandleScoreChanged;
    }

    void OnDisable()
    {
        GameManager.OnGameOver    -= TriggerShake;
        ScoreManager.OnScoreChanged -= HandleScoreChanged;
    }

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera) originalCamLocalPos = targetCamera.transform.localPosition;
    }

    void HandleScoreChanged(int score)
    {
        // Every 5 blocks: milestone burst
        if (score > 0 && score % 5 == 0 && milestoneParticles)
        {
            milestoneParticles.transform.position = targetCamera.transform.position + Vector3.forward * 5f;
            milestoneParticles.Play();
        }
    }

    // Call this from StackController on perfect placement
    public void TriggerPerfectBurst(Vector3 worldPos)
    {
        if (perfectParticles == null) return;
        perfectParticles.transform.position = worldPos;
        perfectParticles.Play();
    }

    // ── Block squish on placement ───────────────────────────────────
    public void SquishBlock(GameObject block)
    {
        if (block != null) StartCoroutine(SquishRoutine(block));
    }

    IEnumerator SquishRoutine(GameObject block)
    {
        if (block == null) yield break;
        Vector3 original = block.transform.localScale;
        Vector3 squished = new Vector3(original.x * 1.08f, original.y * 0.72f, original.z);

        float t = 0f;
        // Squish down
        while (t < 1f)
        {
            t += Time.deltaTime / 0.05f;
            if (block == null) yield break;
            block.transform.localScale = Vector3.Lerp(original, squished, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        t = 0f;
        // Bounce back
        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            if (block == null) yield break;
            block.transform.localScale = Vector3.Lerp(squished, original, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        if (block != null) block.transform.localScale = original;
    }

    void TriggerShake()
    {
        if (!isShaking) StartCoroutine(ShakeRoutine());
    }

    System.Collections.IEnumerator ShakeRoutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / shakeDuration);
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude * t;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude * t;

            if (targetCamera)
                targetCamera.transform.localPosition = originalCamLocalPos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        if (targetCamera)
            targetCamera.transform.localPosition = originalCamLocalPos;
        isShaking = false;
    }
}
