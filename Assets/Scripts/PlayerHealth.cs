using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;

    private int currentHealth;
    private bool isDead;

    private Vector3 startingPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        startingPosition = transform.position;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        PlayerController controller = GetComponent<PlayerController>();

        // Dodge gives temporary invulnerability.
        if (controller != null && controller.IsDodging)
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

    private void Respawn()
    {
        transform.position = startingPosition;

        currentHealth = maxHealth;
        isDead = false;
    }
}
