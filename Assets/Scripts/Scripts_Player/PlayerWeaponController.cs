using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    public enum WeaponType
    {
        Broom,
        Mop,
        Sponge
    }

    [System.Serializable]
    public class WeaponSettings
    {
        [Header("Weapon")]
        public WeaponType weaponType;

        [Header("Attack")]
        [Tooltip("Maximum distance the attack can reach.")]
        public float range = 2.5f;

        [Tooltip("How much cleaning damage one successful hit does.")]
        public float cleaningDamage = 20f;

        [Tooltip("Time between attacks.")]
        public float attackCooldown = 0.35f;

        [Header("Mop Arc")]
        [Tooltip("Only used by the Mop.")]
        [Range(1f, 360f)]
        public float attackArc = 90f;

        [Header("Broom")]
        [Tooltip("How wide the broom's straight-line attack is.")]
        public float attackWidth = 0.75f;
    }

    [Header("Current Weapon")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Mop;

    [Header("Weapon Settings")]
    [SerializeField]
    private WeaponSettings broom = new WeaponSettings
    {
        weaponType = WeaponType.Broom,
        range = 3.5f,
        cleaningDamage = 10f,
        attackCooldown = 0.45f,
        attackWidth = 0.75f
    };

    [SerializeField]
    private WeaponSettings mop = new WeaponSettings
    {
        weaponType = WeaponType.Mop,
        range = 2.5f,
        cleaningDamage = 20f,
        attackCooldown = 0.35f,
        attackArc = 90f
    };

    [SerializeField]
    private WeaponSettings sponge = new WeaponSettings
    {
        weaponType = WeaponType.Sponge,
        range = 1.5f,
        cleaningDamage = 35f,
        attackCooldown = 0.65f
    };

    [Header("Attack")]
    [SerializeField] private LayerMask cleaningTargetLayer;

    [Header("Visual")]
    [SerializeField] private Transform attackOrigin;

    private float attackCooldownTimer;

    private Vector2 facingDirection = Vector2.right;

    public WeaponType CurrentWeapon => currentWeapon;

    private void Update()
    {
        UpdateFacingDirection();

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        ReadInput();
    }

    private void UpdateFacingDirection()
    {
        if (Keyboard.current == null)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        if (input != Vector2.zero)
        {
            facingDirection = input.normalized;
        }
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        // Left mouse button attacks.
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }

        // Weapon selection.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentWeapon = WeaponType.Broom;
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentWeapon = WeaponType.Mop;
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentWeapon = WeaponType.Sponge;
        }
    }

    private void TryAttack()
    {
        if (attackCooldownTimer > 0f)
            return;

        WeaponSettings weapon = GetCurrentWeaponSettings();

        if (weapon == null)
            return;

        attackCooldownTimer = weapon.attackCooldown;

        switch (currentWeapon)
        {
            case WeaponType.Broom:
                PerformBroomAttack(weapon);
                break;

            case WeaponType.Mop:
                PerformMopAttack(weapon);
                break;

            case WeaponType.Sponge:
                PerformSpongeAttack(weapon);
                break;
        }
    }

    private WeaponSettings GetCurrentWeaponSettings()
    {
        switch (currentWeapon)
        {
            case WeaponType.Broom:
                return broom;

            case WeaponType.Mop:
                return mop;

            case WeaponType.Sponge:
                return sponge;
        }

        return null;
    }

    private Vector2 GetAttackOrigin()
    {
        if (attackOrigin != null)
            return attackOrigin.position;

        return transform.position;
    }

    private void PerformBroomAttack(WeaponSettings weapon)
    {
        Vector2 origin = GetAttackOrigin();

        Vector2 center =
            origin + facingDirection * (weapon.range * 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            new Vector2(
                weapon.range,
                weapon.attackWidth
            ),
            Vector2.SignedAngle(Vector2.right, facingDirection),
            cleaningTargetLayer
        );

        ApplyCleaningDamage(hits, weapon.cleaningDamage);
    }

    private void PerformMopAttack(WeaponSettings weapon)
    {
        Vector2 origin = GetAttackOrigin();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            weapon.range,
            cleaningTargetLayer
        );

        List<Collider2D> validHits = new List<Collider2D>();

        foreach (Collider2D hit in hits)
        {
            Vector2 directionToTarget =
                ((Vector2)hit.transform.position - origin).normalized;

            float angle = Vector2.Angle(
                facingDirection,
                directionToTarget
            );

            if (angle <= weapon.attackArc * 0.5f)
            {
                validHits.Add(hit);
            }
        }

        ApplyCleaningDamage(
            validHits.ToArray(),
            weapon.cleaningDamage
        );
    }

    private void PerformSpongeAttack(WeaponSettings weapon)
    {
        Vector2 origin = GetAttackOrigin();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            weapon.range,
            cleaningTargetLayer
        );

        ApplyCleaningDamage(hits, weapon.cleaningDamage);
    }

    private void ApplyCleaningDamage(
        Collider2D[] hits,
        float damage)
    {
        HashSet<CleaningTarget> alreadyHit =
            new HashSet<CleaningTarget>();

        foreach (Collider2D hit in hits)
        {
            CleaningTarget target =
                hit.GetComponentInParent<CleaningTarget>();

            if (target == null)
                continue;

            // Prevent the same object from taking damage twice
            // if it has multiple colliders.
            if (alreadyHit.Contains(target))
                continue;

            alreadyHit.Add(target);

            target.TakeCleaningDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        WeaponSettings weapon = GetCurrentWeaponSettings();

        if (weapon == null)
            return;

        Vector3 origin =
            attackOrigin != null
                ? attackOrigin.position
                : transform.position;

        Gizmos.DrawWireSphere(origin, weapon.range);
    }
}