using UnityEngine;
using System.Collections;

public class HazardProjectile : MonoBehaviour
{
    [Header("Base Settings")]
    public float speed = 2f;
    public float lifetime = 20f;
    public float damage = 10f;

    private Vector2 moveDirection;
    private bool isPiercing = false;
    private bool isReturning = false;
    private Transform returnTarget;

    public void Initialize(Vector2 direction, float customSpeed, float customDamage)
    {
        moveDirection = direction.normalized;
        speed = customSpeed;
        damage = customDamage;
        //isPiercing = piercing;
        //isReturning = returning;
        //returnTarget = target;

        Destroy(gameObject, lifetime);
        StartCoroutine(ProjectileBehaviorRoutine());
    }

    private IEnumerator ProjectileBehaviorRoutine()
    {
        // Non-returning projectiles
        if (!isReturning)
        {
            while (true)
            {
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
                yield return null;
            }
        }

        // Returning projectiles
        float timer = 0f;
        float outwardDuration = 2f; // Time spent traveling outward before returning

        // Phase 1: Travel outward
        while (timer < outwardDuration)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Mjolnir boomerang return logic
        if (isReturning)
        {
            yield return new WaitForSeconds(0.2f); // Brief apex pause before snapping back

            while (true)
            {
                if (returnTarget != null)
                {
                    // Track back toward the spawner/paladin
                    Vector2 returnDir = (returnTarget.position - transform.position).normalized;
                    transform.Translate(returnDir * speed * Time.deltaTime, Space.World);

                    // Despawn if it successfully reaches the source
                    if (Vector2.Distance(transform.position, returnTarget.position) < 0.6f)
                    {
                        Destroy(gameObject);
                        yield break;
                    }
                }
                else
                {
                    // Fallback if target is gone: reverse straight back
                    transform.Translate(-moveDirection * speed * Time.deltaTime, Space.World);
                }
                yield return null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the player's cleaning tool so it doesn't prematurely destroy projectiles
        if (other.CompareTag("CleaningTool"))
        {
            return;
        }

        // Ignore other projectiles
        if (other.GetComponent<HazardProjectile>() != null)
            return;

        if (other.CompareTag("Player"))
        {

            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }

        // Destroy on impact with valid targets unless it is piercing
        if (!isPiercing)
        {
            Destroy(gameObject);
        }
    }
}