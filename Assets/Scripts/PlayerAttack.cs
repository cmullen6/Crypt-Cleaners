
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackDuration = 0.16f;
    [SerializeField] private float hitboxDistance = 0.8f;
    [SerializeField] private Vector2 hitboxSize = new Vector2(1f, 1f);
    [SerializeField] private LayerMask targetLayers;

    [Header("Combo Settings")]
    [SerializeField] private bool enableCombo = true;
    [SerializeField, Min(1)] private int comboSteps = 3;
    [SerializeField] private float comboWindow = 0.65f;

    [Header("Hitstop Settings")]
    [SerializeField] private float trashHitstop = 0.06f;
    [SerializeField] private float otherHitstop = 0.04f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private PlayerController playerController;

    private float attackTimer;
    private float comboTimer;

    private int comboIndex;

    private bool isAttacking;
    private bool comboQueued;

    // Tracks colliders already hit during the current attack.
    // This avoids using the obsolete GetInstanceID() method.
    private readonly HashSet<Collider2D> hitTargets =
        new HashSet<Collider2D>();

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Don't advance combat while gameplay is frozen.
        if (Time.timeScale == 0f)
            return;

        UpdateComboTimer();

        if (Mouse.current == null)
            return;

        // Left mouse button starts an attack or queues a combo hit.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isAttacking)
            {
                if (enableCombo && comboIndex < comboSteps)
                    comboQueued = true;
            }
            else
            {
                StartAttack();
            }
        }

        if (!isAttacking)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
            FinishAttack();
    }

    private void UpdateComboTimer()
    {
        if (comboTimer <= 0f)
            return;

        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0f)
        {
            comboTimer = 0f;
            comboIndex = 0;
            comboQueued = false;
        }
    }

    private void StartAttack()
    {
        if (playerController == null)
            return;

        if (playerController.IsDodging ||
            playerController.IsInvincible)
            return;

        isAttacking = true;
        attackTimer = attackDuration;

        comboIndex++;

        // Each swing can hit the same target again.
        hitTargets.Clear();

        Vector2 direction = playerController.FacingDirection;

        Vector2 hitboxCenter =
            (Vector2)transform.position +
            direction * hitboxDistance;

        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetInteger("AttackCombo", comboIndex);
        }

        PerformAttack(hitboxCenter, angle);
    }


    private void PerformAttack(Vector2 center, float angle)
    {
        // Find every collider inside the attack hitbox on the selected layers.
        Collider2D[] results = Physics2D.OverlapBoxAll(
            center,
            hitboxSize,
            angle,
            targetLayers
        );

        bool hitTrash = false;
        bool hitOther = false;

        foreach (Collider2D target in results)
        {
            if (target == null)
                continue;

            // Prevent the attack from hitting the player or their own colliders.
            if (target.transform == transform ||
                target.transform.IsChildOf(transform))
            {
                continue;
            }

            // Prevent the same collider from being processed twice per swing.
            if (!hitTargets.Add(target))
                continue;

            // Call TakeDamage(int) on the target or one of its parent objects.
            target.SendMessageUpwards(
                "TakeDamage",
                attackDamage,
                SendMessageOptions.DontRequireReceiver
            );

            // Check whether the collider or its root is tagged as Trash.
            bool isTrash =
                target.CompareTag("Trash") ||
                target.transform.root.CompareTag("Trash");

            if (isTrash)
                hitTrash = true;
            else
                hitOther = true;
        }

        // Briefly freeze gameplay when the attack connects.
        if (HitStopManager.Instance != null)
        {
            if (hitTrash)
                HitStopManager.Instance.StopTime(trashHitstop);
            else if (hitOther)
                HitStopManager.Instance.StopTime(otherHitstop);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        // Start the next swing if the player queued a combo input.
        if (comboQueued &&
            enableCombo &&
            comboIndex < comboSteps)
        {
            comboQueued = false;
            StartAttack();
        }
        else
        {
            comboQueued = false;
            comboTimer = comboWindow;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = Vector2.right;

        if (playerController != null)
            direction = playerController.FacingDirection;

        Vector2 center =
            (Vector2)transform.position +
            direction * hitboxDistance;

        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(
            center,
            Quaternion.Euler(0f, 0f, angle),
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero, hitboxSize);

        Gizmos.matrix = previousMatrix;
    }
}
