using UnityEngine;

public class HazardProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private int damage = 1;

    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void Initialize(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    public void Initialize(Vector2 newDirection, float newSpeed, int newDamage)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
