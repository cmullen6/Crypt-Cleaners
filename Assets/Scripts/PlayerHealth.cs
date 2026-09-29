using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 15;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;

    private int currentHealth;
    public bool isDead;

    private Vector3 startingPosition;
    private PlayerController playerController;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        startingPosition = transform.position;
        currentHealth = maxHealth;

        // Cache reference to avoid GetComponent allocations on hit
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // Checks the independent iFrames timer on PlayerController
        if (playerController != null && playerController.IsInvincible)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Invoke(nameof(Respawn), respawnDelay);
    }

    public void Respawn()
    {
        transform.position = startingPosition;

        currentHealth = maxHealth;
        isDead = false;
    }
}
