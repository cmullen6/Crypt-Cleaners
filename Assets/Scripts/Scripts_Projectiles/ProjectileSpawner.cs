using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Pattern Sequencing")]
    [SerializeField] private BulletPatternData[] patternSequence;
    private int currentPatternIndex = 0;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fallbackProjectilePrefab; // Used if a ScriptableObject prefab field is empty

    [Header("Automation Settings")]
    [SerializeField] private float attackInterval = 2.0f; // Make sure to have it at the very least match the duration of pattern.
                                                          // Any less will result in more than one of the same pattern in that duration

    private Coroutine activeSpiralRoutine; // Safety check to prevent multiple routines

    private void Start()
    {
        if (firePoint == null)
        {
            firePoint = transform;
        }

        if (patternSequence != null && patternSequence.Length > 0)
        {
            StartCoroutine(AutomatedSequenceRoutine());
        }
        else
        {
            Debug.LogWarning("No pattern sequence assigned to ProjectileSpawner!", this);
        }
    }

    private IEnumerator AutomatedSequenceRoutine()
    {
        yield return new WaitForSeconds(0.5f); // Initial startup delay

        while (true)
        {
            BulletPatternData currentPatternData = patternSequence[currentPatternIndex];

            if (currentPatternData != null)
            {
                ExecuteAttack(currentPatternData);
            }

            // Advance index, looping back to the start when reaching the end of the array
            currentPatternIndex = (currentPatternIndex + 1) % patternSequence.Length;

            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void ExecuteAttack(BulletPatternData patternData)
    {
        if (patternData is LinearPatternData linearData)
        {
            HandleLinear(linearData);
        }
        else if (patternData is MeleeAreaPatternData meleeData)
        {
            HandleMeleeArea(meleeData);
        }
        else if (patternData is RadialBurstPatternData radialData)
        {
            HandleRadialBurst(radialData);
        }
        else if (patternData is SpiralPatternData spiralData)
        {
            HandleSpiral(spiralData);
        }
        else if (patternData is SpreadPatternData spreadData)
        {
            HandleSpread(spreadData);
        }
    }

    public void ExecuteAttack()
    {
        if (patternSequence != null && patternSequence.Length > 0)
        {
            ExecuteAttack(patternSequence[currentPatternIndex]);
        }
    }

    // Helper to get the correct prefab from data, falling back if necessary
    private GameObject GetPrefabToSpawn(BulletPatternData data, GameObject specificPrefab = null)
    {
        if (specificPrefab != null) return specificPrefab;
        if (data != null && data.ProjectilePrefab != null) return data.ProjectilePrefab;
        return fallbackProjectilePrefab;
    }

    //////////////////// 
    /// HANDLER METHODS
    //////////////////// 

    private void HandleLinear(LinearPatternData data)
    {
        StartCoroutine(FireLinearPatternRoutine(data));
    }

    private IEnumerator FireLinearPatternRoutine(LinearPatternData data)
    {
        int shotsToFire = data.isBurst ? Mathf.Max(1, data.burstCount) : 1;
        float delayBetweenShots = data.isBurst ? data.burstInterval : 0f;

        for (int i = 0; i < shotsToFire; i++)
        {
            GameObject prefabToUse = GetPrefabToSpawn(data);
            if (prefabToUse != null)
            {
                Vector2 direction = transform.up;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                GameObject spawnedProjectile = Instantiate(prefabToUse, firePoint.position, rotation);

                // Apply scale
                spawnedProjectile.transform.localScale = Vector3.one * data.ProjectileScale;

                // Check if it's a standard projectile
                HazardProjectile hazard = spawnedProjectile.GetComponent<HazardProjectile>();
                if (hazard != null)
                {
                    hazard.Initialize(
                        direction,
                        data.ProjectileSpeed,
                        data.Damage,
                        data.IsPiercing,
                        data.IsReturning,
                        this.transform
                    );
                }
                else
                {
                    // Check if it's a Laser Hazard (Holy Barrier)
                    LaserHazard laser = spawnedProjectile.GetComponent<LaserHazard>();
                    if (laser != null)
                    {
                        laser.InitializeLaser(data);
                    }
                }
            }

            if (i < shotsToFire - 1 && delayBetweenShots > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShots);
            }
        }
    }

    private void HandleMeleeArea(MeleeAreaPatternData data)
    {
        // Melee hazards use AreaAttackPrefab specifically defined on their data asset
        if (data.AreaAttackPrefab != null)
        {
            HeroAttackHazard spawnedMelee = Instantiate(data.AreaAttackPrefab, firePoint.position, firePoint.rotation);

            if (data.AttackShape == MeleeAreaPatternData.MeleeShape.Rectangle)
            {
                // Hammer Cleave (Rectangle)
                spawnedMelee.InitializeRectangle(
                    data.ShapeSizeRadiusOrWidth,         // Width
                    data.ShapeSizeLengthOrOffset,        // Length
                    data.ShapeSizeLengthOrOffset * 0.5f, // Forward Offset
                    data.AreaAttackDuration,
                    data.Damage
                );
            }
            else
            {
                // Axe Circle (Circle)
                spawnedMelee.InitializeCircle(
                    data.ShapeSizeRadiusOrWidth,         // Radius
                    data.AreaAttackDuration,
                    data.Damage
                );
            }
        }
    }


    private void HandleRadialBurst(RadialBurstPatternData data)
    {
        GameObject prefabToUse = GetPrefabToSpawn(data);
        if (prefabToUse == null) return;

        float angleStep = 360f / data.ProjectileCount;
        float currentAngle = 0f;

        for (int i = 0; i < data.ProjectileCount; i++)
        {
            float angleRad = currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject spawnedProjectile = Instantiate(prefabToUse, firePoint.position, rotation);
            HazardProjectile hazard = spawnedProjectile.GetComponent<HazardProjectile>();

            if (hazard != null)
            {
                hazard.Initialize(direction, data.ProjectileSpeed, 10);
            }

            currentAngle += angleStep;
        }
    }

    private void HandleSpread(SpreadPatternData data)
    {
        GameObject prefabToUse = GetPrefabToSpawn(data);
        if (prefabToUse == null) return;

        float centerAngle = firePoint.eulerAngles.z;
        float startAngle = centerAngle - (data.SpreadAngle / 2f);
        float angleStep = data.ProjectileCount > 1 ? data.SpreadAngle / (data.ProjectileCount - 1) : 0f;

        for (int i = 0; i < data.ProjectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            float angleRad = currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject spawnedProjectile = Instantiate(prefabToUse, firePoint.position, rotation);

            // Apply scale
            spawnedProjectile.transform.localScale = Vector3.one * data.ProjectileScale;

            HazardProjectile hazard = spawnedProjectile.GetComponent<HazardProjectile>();

            if (hazard != null)
            {
                // Now correctly passes the Speed, Damage, and flags from SpreadPatternData
                hazard.Initialize(
                    direction,
                    data.ProjectileSpeed,
                    data.Damage,
                    data.IsPiercing,
                    false,
                    null
                );
            }
        }
    }

    private void HandleSpiral(SpiralPatternData data)
    {
        // If a spiral is already running, stop it so they don't overlap
        if (activeSpiralRoutine != null)
        {
            StopCoroutine(activeSpiralRoutine);
        }

        StartCoroutine(RunSpiral(data));
    }

    private IEnumerator RunSpiral(BulletPatternData basePattern)
    {
        // Cast base pattern to SpiralPatternData so we can access spiral properties
        if (basePattern is not SpiralPatternData pattern) yield break;

        float elapsed = 0f;
        float currentAngle = 0f;

        float fireInterval = Mathf.Max(
            0.01f,
            pattern.FireInterval
        );

        while (elapsed < pattern.Duration)
        {
            float radians = currentAngle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            SpawnProjectile(
                direction,
                4f,
                pattern.Damage,
                pattern
            );

            currentAngle += pattern.SpiralRotation;

            yield return new WaitForSeconds(fireInterval);

            elapsed += fireInterval;
        }
    }

    // Helper method to handle the actual instantiation using multi-prefab system
    private void SpawnProjectile(Vector2 direction, float speed, int damage, BulletPatternData patternData)
    {
        GameObject prefabToUse = GetPrefabToSpawn(patternData);
        if (prefabToUse == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject spawnedProjectile = Instantiate(prefabToUse, firePoint.position, rotation);
        HazardProjectile hazard = spawnedProjectile.GetComponent<HazardProjectile>();

        if (hazard != null)
        {
            hazard.Initialize(direction, speed, damage);
        }
    }
}
