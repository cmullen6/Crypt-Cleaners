
using System.Collections.Generic;
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

    [Header("Resource Drops")]
    [SerializeField, Range(0f, 100f)]
    private float dropChance = 100f;

    [SerializeField, Range(1, 2)]
    private int minResourceTypes = 1;

    [SerializeField, Range(1, 2)]
    private int maxResourceTypes = 2;

    [SerializeField]
    private ResourcePickup.RewardDestination rewardDestination =
        ResourcePickup.RewardDestination.CurrentRun;

    [SerializeField, Min(0f)]
    private float dropRadius = 0.5f;

    [SerializeField]
    private List<ResourceDropOption> possibleDrops =
        new List<ResourceDropOption>();

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

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;

        UpdateProgressBar();

        if (CleaningManager.Instance != null)
            CleaningManager.Instance.RegisterCleaningSpot(this);
    }

    public void Clean(float amount)
    {
        if (isCleaned || amount <= 0f)
            return;

        cleaningProgress += amount;

        cleaningProgress = Mathf.Clamp(
            cleaningProgress,
            0f,
            cleaningRequired
        );

        UpdateProgressBar();

        if (cleaningProgress >= cleaningRequired)
            CompleteCleaning();
    }

    public void UpdateProgressBar()
    {
        if (progressBar != null)
            progressBar.value = Progress01;
    }

    private void CompleteCleaning()
    {
        if (isCleaned)
            return;

        isCleaned = true;

        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        PlaySparkleEffect();

        // Generate randomized resource rewards.
        DropResources();

        if (CleaningManager.Instance != null)
            CleaningManager.Instance.SpotCleaned(this);
    }

    // --------------------------------------------------
    // RESOURCE DROP SYSTEM
    // --------------------------------------------------

    private void DropResources()
    {
        // Check whether this spot should drop resources.
        if (Random.Range(0f, 100f) > dropChance)
            return;

        // Build a list of valid resource options.
        // Each resource type can only appear once.
        List<ResourceDropOption> availableDrops =
            new List<ResourceDropOption>();

        HashSet<ResourceType> usedTypes =
            new HashSet<ResourceType>();

        foreach (ResourceDropOption option in possibleDrops)
        {
            if (option == null)
                continue;

            if (!option.enabled || option.pickupPrefab == null)
                continue;

            if (usedTypes.Contains(option.resourceType))
                continue;

            // Make sure the prefab has a ResourcePickup component.
            if (option.pickupPrefab.GetComponent<ResourcePickup>() == null)
            {
                Debug.LogWarning(
                    "Resource pickup prefab for " +
                    option.resourceType +
                    " is missing a ResourcePickup component.",
                    this
                );

                continue;
            }

            availableDrops.Add(option);
            usedTypes.Add(option.resourceType);
        }

        // Nothing available to drop.
        if (availableDrops.Count == 0)
            return;

        // Determine how many different resource types to drop.
        int minimum = Mathf.Clamp(
            minResourceTypes, 1, availableDrops.Count
        );

        int maximum = Mathf.Clamp(
            maxResourceTypes, minimum, availableDrops.Count
        );

        int numberOfTypes = Random.Range(
            minimum,
            maximum + 1
        );

        // Shuffle the available resources.
        // This ensures that we select different types.
        for (int i = 0; i < availableDrops.Count; i++)
        {
            int randomIndex = Random.Range(
                i,
                availableDrops.Count
            );

            ResourceDropOption temporary = availableDrops[i];

            availableDrops[i] = availableDrops[randomIndex];

            availableDrops[randomIndex] = temporary;
        }

        // Spawn the selected resource pickups.
        for (int i = 0; i < numberOfTypes; i++)
        {
            SpawnResource(availableDrops[i]);
        }
    }


    private void SpawnResource(ResourceDropOption option)
    {
        if (option == null || option.pickupPrefab == null)
            return;

        int minimumAmount = Mathf.Max(1, option.minimumAmount);

        int maximumAmount = Mathf.Max(
            minimumAmount,
            option.maximumAmount
        );

        int amount = Random.Range(
            minimumAmount,
            maximumAmount + 1
        );

        // Give each resource a slightly different spawn position.
        Vector2 randomOffset =
            Random.insideUnitCircle * 0.15f;

        Vector3 spawnPosition = transform.position +
            new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject pickup = Instantiate(
            option.pickupPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ResourcePickup resourcePickup =
            pickup.GetComponent<ResourcePickup>();

        if (resourcePickup != null)
        {
            // Configure the resource data and sprite.
            resourcePickup.ConfigurePickup(
                option.resourceType,
                amount,
                rewardDestination
            );

            // Launch the resource outward.
            resourcePickup.LaunchPickup();
        }
        else
        {
            Debug.LogError(
                "Resource pickup prefab is missing ResourcePickup!",
                pickup
            );

            Destroy(pickup);
        }
    }


    // --------------------------------------------------
    // CLEANING EFFECTS
    // --------------------------------------------------

    private void PlaySparkleEffect()
    {
        if (sparklePrefab == null)
            return;

        Vector3 effectPosition = transform.position;

        if (spriteRenderer != null)
            effectPosition = spriteRenderer.bounds.center;

        ParticleSystem sparkle = Instantiate(
            sparklePrefab,
            effectPosition,
            Quaternion.identity
        );

        sparkle.Play();

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

    private void OnValidate()
    {
        minResourceTypes = Mathf.Clamp(minResourceTypes, 1, 2);
        maxResourceTypes = Mathf.Clamp(
            maxResourceTypes,
            minResourceTypes,
            2
        );
    }
}

// --------------------------------------------------
// RESOURCE DROP CONFIGURATION
// --------------------------------------------------

[System.Serializable]
public class ResourceDropOption
{
    [Tooltip("Whether this resource can drop.")]
    public bool enabled = true;

    [Tooltip("The type of resource to spawn.")]
    public ResourceType resourceType;

    [Tooltip("The pickup prefab for this resource.")]
    public GameObject pickupPrefab;

    [Tooltip("Minimum amount spawned.")]
    [Min(1)]
    public int minimumAmount = 1;

    [Tooltip("Maximum amount spawned.")]
    [Min(1)]
    public int maximumAmount = 3;
}

