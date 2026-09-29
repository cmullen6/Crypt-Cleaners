using UnityEngine;

public class HeroAttackHazard : MonoBehaviour
{
    public enum HazardShape
    {
        Circle,
        Rectangle
    }

    [Header("Colliders")]
    [SerializeField] private CircleCollider2D circleCollider;

    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int damage;
    private float lifetime;
    private bool hasHitPlayer;

    // ---------------------------------------------------------
    // BARBARIAN CIRCLE ATTACK
    // ---------------------------------------------------------

    public void InitializeCircle(
        float radius,
        float duration,
        int newDamage)
    {
        damage = newDamage;
        lifetime = duration;
        hasHitPlayer = false;

        if (circleCollider != null)
        {
            circleCollider.enabled = true;
            circleCollider.radius = radius;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.transform.localScale =
                new Vector3(
                    radius * 2f,
                    radius * 2f,
                    1f
                );

            spriteRenderer.transform.localPosition =
                Vector3.zero;
        }

        Destroy(gameObject, lifetime);
    }

    // ---------------------------------------------------------
    // PALADIN RECTANGLE ATTACK
    // ---------------------------------------------------------

    public void InitializeRectangle(
        float width,
        float length,
        float forwardOffset,
        float duration,
        int newDamage)
    {
        damage = newDamage;
        lifetime = duration;
        hasHitPlayer = false;

        if (circleCollider != null)
        {
            circleCollider.enabled = false;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = true;

            boxCollider.size =
                new Vector2(
                    width,
                    length
                );

            boxCollider.offset =
                new Vector2(
                    0f,
                    forwardOffset
                );
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.transform.localScale =
                new Vector3(
                    width,
                    length,
                    1f
                );

            spriteRenderer.transform.localPosition =
                new Vector3(
                    0f,
                    forwardOffset,
                    0f
                );
        }

        Destroy(gameObject, lifetime);
    }

    // ---------------------------------------------------------
    // PLAYER HIT
    // ---------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            hasHitPlayer = true;
        }
    }
}