
using UnityEngine;
using UnityEngine.UI;

public class CleaningSpot : MonoBehaviour
{
    [Header("Cleaning Settings")]
    [SerializeField, Min(0.01f)]
    private float cleaningRequired = 100f;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite dirtySprite;
    [SerializeField] private Sprite cleanSprite;

    [Header("Clean Effect")]
    [SerializeField] private ParticleSystem sparklePrefab;

    [Header("Progress Bar")]
    [SerializeField] private Slider progressBar;

    private float cleaningProgress;
    private bool isCleaned;

    public float Progress01
    {
        get
        {
            if (cleaningRequired <= 0f)
                return 1f;

            return Mathf.Clamp01(cleaningProgress / cleaningRequired);
        }
    }

    public bool IsCleaned => isCleaned;

    private void Start()
    {
        cleaningProgress = 0f;
        isCleaned = false;

        // Find a SpriteRenderer if one was not assigned.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Start with the dirty appearance.
        if (spriteRenderer != null && dirtySprite != null)
        {
            spriteRenderer.sprite = dirtySprite;
        }

        UpdateProgressBar();

        // Register this object with the room cleaning manager.
        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.RegisterCleaningSpot(this);
        }
    }

    public void Clean(float amount)
    {
        // Do not allow an already-clean object to be cleaned again.
        if (isCleaned)
            return;

        // Ignore invalid cleaning amounts.
        if (amount <= 0f)
            return;

        cleaningProgress += amount;

        cleaningProgress = Mathf.Clamp(
            cleaningProgress,
            0f,
            cleaningRequired
        );

        UpdateProgressBar();

        // Check whether this object has been fully cleaned.
        if (cleaningProgress >= cleaningRequired)
        {
            CompleteCleaning();
        }
    }

    public void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.value = Progress01;
        }
    }

    private void CompleteCleaning()
    {
        if (isCleaned)
            return;

        isCleaned = true;

        // Change the dirty sprite into the clean sprite.
        if (spriteRenderer != null && cleanSprite != null)
        {
            spriteRenderer.sprite = cleanSprite;
        }

        // Play sparkle effect at the center of the sprite.
        PlaySparkleEffect();

        // Notify the room cleaning manager.
        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.SpotCleaned(this);
        }

        // Keep this object active so the clean sprite remains visible.
    }

    private void PlaySparkleEffect()
    {
        if (sparklePrefab == null)
            return;

        Vector3 effectPosition = transform.position;

        // Use the visual center of the sprite when possible.
        if (spriteRenderer != null)
        {
            effectPosition = spriteRenderer.bounds.center;
        }

        ParticleSystem sparkle = Instantiate(
            sparklePrefab,
            effectPosition,
            Quaternion.identity
        );

        sparkle.Play();

        // Destroy the effect after its particles finish.
        float lifetime = sparkle.main.duration;

        if (sparkle.main.startLifetime.mode ==
            ParticleSystemCurveMode.Constant)
        {
            lifetime += sparkle.main.startLifetime.constant;
        }
        else
        {
            lifetime += sparkle.main.startLifetime.constantMax;
        }

        Destroy(sparkle.gameObject, lifetime + 0.5f);
    }
}

