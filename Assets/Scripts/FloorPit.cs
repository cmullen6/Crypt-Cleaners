using System.Collections;
using UnityEngine;

public class FloorPit : MonoBehaviour
{
    [SerializeField] private int fallDamage = 1;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float respawnDelay = 0.2f;

    private Transform playerTransform;
    private Collider2D playerFeetCollider;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private Collider2D pitCollider;

    private Vector3 lastSafePosition;
    private float recordTimer;
    private bool isFalling;

    private void Awake()
    {
        pitCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        // Automatically find the player using the PlayerFeet tag
        GameObject feetObj = GameObject.FindGameObjectWithTag("PlayerFeet");
        if (feetObj != null)
        {
            playerFeetCollider = feetObj.GetComponent<Collider2D>();
            playerRb = feetObj.GetComponentInParent<Rigidbody2D>();
            playerController = feetObj.GetComponentInParent<PlayerController>();
            playerTransform = playerRb != null ? playerRb.transform : feetObj.transform.root;

            lastSafePosition = playerTransform.position;
        }
    }

    private void Update()
    {
        if (playerTransform == null || isFalling) return;

        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;

            // Only record the position if PlayerFeet is NOT touching this pit
            if (playerFeetCollider != null && !pitCollider.IsTouching(playerFeetCollider))
            {
                lastSafePosition = playerTransform.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFeet") && !isFalling)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(fallDamage);
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isFalling = true;

        // Disable movement controls immediately
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Freeze momentum so the player stops sliding
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero; 
        }

        // Wait briefly for any fall animation or effect
        yield return new WaitForSeconds(respawnDelay);

        // Teleport back to the exact spot recorded right before falling
        if (playerTransform != null)
        {
            playerTransform.position = lastSafePosition;
        }

        // Re-enable movement controls
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isFalling = false;
    }
}