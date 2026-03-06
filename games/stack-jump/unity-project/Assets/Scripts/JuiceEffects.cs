using UnityEngine;

/// <summary>
/// Spawns particle bursts on score milestones and screen shake on game over.
/// Attach to a persistent GameObject alongside GameManager.
/// </summary>
public class JuiceEffects : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem milestoneParticles;   // Burst at every 5 blocks
    public ParticleSystem perfectParticles;     // Burst on perfect placement

    [Header("Screen Shake")]
    public Camera targetCamera;
    [Range(0f, 1f)] public float shakeDuration  = 0.25f;
    [Range(0f, 1f)] public float shakeMagnitude = 0.15f;

    private Vector3 originalCamLocalPos;
    private bool    isShaking;

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
