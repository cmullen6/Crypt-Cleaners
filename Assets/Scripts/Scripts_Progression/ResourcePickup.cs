
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public enum RewardDestination
    {
        BankedImmediately,
        CurrentRun
    }

    [Header("Reward Data")]
    public ResourceType resourceType = ResourceType.Gold;

    [Min(1)]
    public int amount = 1;

    public RewardDestination destination =
        RewardDestination.CurrentRun;

    [Header("Resource Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite resourceASprite;
    [SerializeField] private Sprite resourceBSprite;
    [SerializeField] private Sprite resourceCSprite;
    [SerializeField] private Sprite resourceDSprite;

    [Header("Loot Explosion")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField, Min(0f)]
    private float minimumLaunchSpeed = 2f;

    [SerializeField, Min(0f)]
    private float maximumLaunchSpeed = 4f;

    [SerializeField, Min(0f)]
    private float launchDamping = 5f;

    [SerializeField]
    private bool randomRotation = true;

    [Header("Collection")]
    public string playerTag = "Player";
    public bool destroyOnCollect = true;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        UpdateVisuals();
    }

    public void ConfigurePickup(
        ResourceType newResourceType,
        int newAmount,
        RewardDestination newDestination)
    {
        resourceType = newResourceType;
        amount = Mathf.Max(1, newAmount);
        destination = newDestination;

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            return;

        Sprite selectedSprite = null;

        switch (resourceType)
        {
            case ResourceType.Gold:
                selectedSprite = goldSprite;
                break;

            case ResourceType.ResourceA:
                selectedSprite = resourceASprite;
                break;

            case ResourceType.ResourceB:
                selectedSprite = resourceBSprite;
                break;

            case ResourceType.ResourceC:
                selectedSprite = resourceCSprite;
                break;

            case ResourceType.ResourceD:
                selectedSprite = resourceDSprite;
                break;
        }

        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning(
                "No sprite assigned for resource: " +
                resourceType,
                this
            );
        }
    }

    // --------------------------------------------------
    // LOOT EXPLOSION
    // --------------------------------------------------

    public void LaunchPickup()
    {
        if (rb == null)
        {
            Debug.LogWarning(
                "ResourcePickup requires a Rigidbody2D!",
                this
            );

            return;
        }

        // Generate a random direction.
        Vector2 direction = Random.insideUnitCircle.normalized;

        // Generate a random launch speed.
        float launchSpeed = Random.Range(
            minimumLaunchSpeed,
            Mathf.Max(minimumLaunchSpeed, maximumLaunchSpeed)
        );

        // Configure physics.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = launchDamping;
        rb.angularDamping = 5f;

        rb.linearVelocity = Vector2.zero;

        // Launch the resource outward.
        rb.AddForce(
            direction * launchSpeed,
            ForceMode2D.Impulse
        );

        // Give the pickup a little spin.
        if (randomRotation)
        {
            rb.AddTorque(
                Random.Range(-100f, 100f),
                ForceMode2D.Impulse
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (destination == RewardDestination.BankedImmediately)
        {
            if (GameProgression.Instance == null)
                return;

            GameProgression.Instance.AddResource(
                resourceType,
                amount
            );
        }
        else
        {
            RunResourceWallet wallet =
                other.GetComponent<RunResourceWallet>();

            if (wallet == null)
            {
                wallet =
                    other.GetComponentInParent<RunResourceWallet>();
            }

            if (wallet == null)
                return;

            wallet.AddResource(resourceType, amount);
        }

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
