
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCleaner : MonoBehaviour
{
    [Header("Mop Attack")]
    [SerializeField, Min(0f)]
    private float mopDamage = 20f;

    [SerializeField, Min(0.01f)]
    private float attackRange = 1.2f;

    [SerializeField, Min(0.01f)]
    private float attackRadius = 0.6f;

    [SerializeField, Min(0.01f)]
    private float attackCooldown = 0.4f;

    [Header("References")]
    [SerializeField]
    private Camera mainCamera;

    [Header("Movement Compatibility")]
    [SerializeField]
    private float cleaningMoveMultiplier = 1f;

    private float nextAttackTime;
    private bool isCleaning;

    private readonly HashSet<CleaningSpot> hitCleaningSpots =
        new HashSet<CleaningSpot>();

    public bool IsCleaning => isCleaning;

    public float MopDamage => mopDamage;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        isCleaning = Mouse.current.leftButton.isPressed;

        if (!isCleaning)
            return;

        if (Time.time < nextAttackTime)
            return;

        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void Attack()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
                return;
        }

        // Convert the mouse position from screen space to world space.
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = transform.position.z;

        // Find the direction from the player toward the mouse.
        Vector2 direction =
            ((Vector2)(mouseWorldPosition - transform.position)).normalized;

        // Prevent a zero direction if the mouse is on the player.
        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.right;

        // Place the attack in front of the player.
        Vector2 attackCenter =
            (Vector2)transform.position + direction * attackRange;

        // Detect everything inside the mop's attack area.
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackCenter,
            attackRadius
        );

        hitCleaningSpots.Clear();

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            // Find a CleaningSpot on the collider or its parent.
            CleaningSpot spot =
                hit.GetComponentInParent<CleaningSpot>();

            if (spot != null && !spot.IsCleaned)
            {
                // Prevent multiple cleaning hits if an object
                // has multiple colliders.
                if (hitCleaningSpots.Add(spot))
                {
                    spot.Clean(mopDamage);
                }
            }

            // Preserve destructible tile cleaning.
            DestructableTiles tiles =
                hit.GetComponentInParent<DestructableTiles>();

            if (tiles != null)
            {
                tiles.EraseTilesAt(
                    attackCenter,
                    attackRadius
                );
            }
        }
    }

    public float GetMovementMultiplier()
    {
        return isCleaning ? cleaningMoveMultiplier : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = Vector2.right;

        if (mainCamera != null && Mouse.current != null)
        {
            Vector3 mouseWorld =
                mainCamera.ScreenToWorldPoint(
                    Mouse.current.position.ReadValue()
                );

            direction =
                ((Vector2)(mouseWorld - transform.position)).normalized;
        }

        Vector2 attackCenter =
            (Vector2)transform.position + direction * attackRange;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, attackCenter);
    }
}
