using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("Projectile")]
    [Tooltip("Normal projectile prefab used by Mage and Archer.")]
    [SerializeField] private HazardProjectile projectilePrefab;

    // =========================================================
    // SEQUENCE SETTINGS
    // =========================================================

    [Header("Sequence")]
    [Tooltip("Time before the first attack begins.")]
    [SerializeField] private float startDelay = 2f;

    [Tooltip("Should the entire attack sequence repeat?")]
    [SerializeField] private bool repeatSequence = true;

    [Tooltip("How many times the entire sequence should play.")]
    [Min(1)]
    [SerializeField] private int sequenceRepeats = 3;

    [Tooltip("The attacks that will play in order.")]
    [SerializeField]
    private List<BulletPatternData> patternSequence =
        new List<BulletPatternData>();

    // =========================================================
    // AIMED TELEGRAPH
    // =========================================================

    [Header("Aimed Attack Telegraph")]
    [SerializeField] private Color telegraphColor = Color.red;

    private Transform playerTransform;

    private LineRenderer aimedTelegraphLine;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        FindPlayer();

        CreateAimedTelegraph();

        StartCoroutine(RunPatternSequence());
    }

    // =========================================================
    // FIND PLAYER
    // =========================================================

    private void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // =========================================================
    // CREATE AIMED TELEGRAPH
    // =========================================================

    private void CreateAimedTelegraph()
    {
        GameObject telegraphObject =
            new GameObject("Aimed Attack Telegraph");

        telegraphObject.transform.SetParent(transform);

        telegraphObject.transform.localPosition =
            Vector3.zero;

        aimedTelegraphLine =
            telegraphObject.AddComponent<LineRenderer>();

        aimedTelegraphLine.positionCount = 2;

        aimedTelegraphLine.useWorldSpace = true;

        aimedTelegraphLine.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        aimedTelegraphLine.startColor =
            telegraphColor;

        aimedTelegraphLine.endColor =
            telegraphColor;

        aimedTelegraphLine.sortingLayerName =
            "Default";

        aimedTelegraphLine.sortingOrder = 10;

        aimedTelegraphLine.enabled = false;
    }

    // =========================================================
    // RUN ENTIRE ATTACK SEQUENCE
    // =========================================================

    private IEnumerator RunPatternSequence()
    {
        if (patternSequence == null ||
            patternSequence.Count == 0)
        {
            Debug.LogWarning(
                "ProjectileSpawner has no Bullet Pattern Data assigned."
            );

            yield break;
        }

        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        int totalSequencePlays;

        if (repeatSequence)
        {
            totalSequencePlays =
                Mathf.Max(1, sequenceRepeats);
        }
        else
        {
            totalSequencePlays = 1;
        }

        for (
            int sequencePlay = 0;
            sequencePlay < totalSequencePlays;
            sequencePlay++
        )
        {
            for (
                int i = 0;
                i < patternSequence.Count;
                i++
            )
            {
                BulletPatternData pattern =
                    patternSequence[i];

                if (pattern == null)
                {
                    Debug.LogWarning(
                        "ProjectileSpawner contains an empty pattern slot."
                    );

                    continue;
                }

                yield return StartCoroutine(
                    ExecutePattern(pattern)
                );

                if (pattern.DelayAfterPattern > 0f)
                {
                    yield return new WaitForSeconds(
                        pattern.DelayAfterPattern
                    );
                }
            }
        }
    }

    // =========================================================
    // CHOOSE PATTERN
    // =========================================================

    private IEnumerator ExecutePattern(
        BulletPatternData pattern)
    {
        switch (pattern.PatternType)
        {
            case BulletPatternData.BulletPatternType.RadialBurst:

                yield return StartCoroutine(
                    RunRadialBurst(pattern)
                );

                break;

            case BulletPatternData.BulletPatternType.Spiral:

                yield return StartCoroutine(
                    RunSpiral(pattern)
                );

                break;

            case BulletPatternData.BulletPatternType.AimedBurst:

                yield return StartCoroutine(
                    RunAimedBurst(pattern)
                );

                break;

            case BulletPatternData.BulletPatternType.AxeCircle:

                yield return StartCoroutine(
                    RunAxeCircle(pattern)
                );

                break;

            case BulletPatternData.BulletPatternType.SwordRectangle:

                yield return StartCoroutine(
                    RunSwordRectangle(pattern)
                );

                break;
        }
    }

    // =========================================================
    // RADIAL BURST
    // =========================================================

    private IEnumerator RunRadialBurst(
        BulletPatternData pattern)
    {
        int burstCount =
            Mathf.Max(
                1,
                pattern.BurstCount
            );

        for (
            int burst = 0;
            burst < burstCount;
            burst++
        )
        {
            SpawnRadialBurst(pattern);

            if (burst < burstCount - 1)
            {
                if (pattern.FireInterval > 0f)
                {
                    yield return new WaitForSeconds(
                        pattern.FireInterval
                    );
                }
            }
        }
    }

    private void SpawnRadialBurst(
        BulletPatternData pattern)
    {
        int projectileCount =
            Mathf.Max(
                1,
                pattern.ProjectileCount
            );

        float angleStep =
            360f / projectileCount;

        for (
            int i = 0;
            i < projectileCount;
            i++
        )
        {
            float angle =
                angleStep * i;

            float radians =
                angle * Mathf.Deg2Rad;

            Vector2 direction =
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

            SpawnProjectile(
                direction,
                pattern.ProjectileSpeed,
                pattern.Damage
            );
        }
    }

    // =========================================================
    // SPIRAL
    // =========================================================

    private IEnumerator RunSpiral(
        BulletPatternData pattern)
    {
        float elapsed = 0f;

        float currentAngle = 0f;

        float fireInterval =
            Mathf.Max(
                0.01f,
                pattern.FireInterval
            );

        while (elapsed < pattern.Duration)
        {
            float radians =
                currentAngle * Mathf.Deg2Rad;

            Vector2 direction =
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

            SpawnProjectile(
                direction,
                pattern.ProjectileSpeed,
                pattern.Damage
            );

            currentAngle +=
                pattern.SpiralRotation;

            yield return new WaitForSeconds(
                fireInterval
            );

            elapsed += fireInterval;
        }
    }

    // =========================================================
    // AIMED BURST
    // =========================================================

    private IEnumerator RunAimedBurst(
        BulletPatternData pattern)
    {
        int burstCount =
            Mathf.Max(
                1,
                pattern.BurstCount
            );

        for (
            int i = 0;
            i < burstCount;
            i++
        )
        {
            if (playerTransform == null)
            {
                FindPlayer();
            }

            if (playerTransform != null)
            {
                // IMPORTANT:
                // This is WORLD-SPACE position math.
                // It prevents sprite flipping from
                // mirroring the attack direction.

                Vector2 direction =
                    (
                        playerTransform.position -
                        transform.position
                    ).normalized;

                yield return StartCoroutine(
                    ShowAimedTelegraph(pattern)
                );

                // Recalculate after the telegraph.
                // This means the shot targets where
                // the player actually is when the shot fires.

                direction =
                    (
                        playerTransform.position -
                        transform.position
                    ).normalized;

                SpawnProjectile(
                    direction,
                    pattern.ProjectileSpeed,
                    pattern.Damage
                );
            }

            if (i < burstCount - 1)
            {
                if (pattern.FireInterval > 0f)
                {
                    yield return new WaitForSeconds(
                        pattern.FireInterval
                    );
                }
            }
        }
    }

    // =========================================================
    // AIMED TELEGRAPH
    // =========================================================

    private IEnumerator ShowAimedTelegraph(
        BulletPatternData pattern)
    {
        if (aimedTelegraphLine == null)
        {
            yield break;
        }

        aimedTelegraphLine.enabled = true;

        aimedTelegraphLine.startWidth =
            pattern.TelegraphWidth;

        aimedTelegraphLine.endWidth =
            pattern.TelegraphWidth;

        float elapsed = 0f;

        while (
            elapsed <
            pattern.TelegraphDuration
        )
        {
            if (playerTransform != null)
            {
                Vector2 direction =
                    (
                        playerTransform.position -
                        transform.position
                    ).normalized;

                Vector3 startPosition =
                    transform.position;

                Vector3 endPosition =
                    startPosition +
                    (
                        (Vector3)direction *
                        pattern.TelegraphLength
                    );

                aimedTelegraphLine.SetPosition(
                    0,
                    startPosition
                );

                aimedTelegraphLine.SetPosition(
                    1,
                    endPosition
                );
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        aimedTelegraphLine.enabled = false;
    }

    // =========================================================
    // BARBARIAN AXE CIRCLE
    // =========================================================

    private IEnumerator RunAxeCircle(
        BulletPatternData pattern)
    {
        if (pattern.AreaAttackPrefab == null)
        {
            Debug.LogWarning(
                "Axe Circle pattern is missing its Area Attack Prefab."
            );

            yield break;
        }

        Vector3 attackPosition =
            transform.position;

        yield return StartCoroutine(
            ShowCircleTelegraph(pattern)
        );

        HeroAttackHazard attack =
            Instantiate(
                pattern.AreaAttackPrefab,
                attackPosition,
                Quaternion.identity
            );

        attack.InitializeCircle(
            pattern.AxeRadius,
            pattern.AreaAttackDuration,
            pattern.Damage
        );
    }

    // =========================================================
    // BARBARIAN CIRCLE TELEGRAPH
    // =========================================================

    private IEnumerator ShowCircleTelegraph(
        BulletPatternData pattern)
    {
        GameObject telegraphObject =
            new GameObject(
                "Barbarian Axe Telegraph"
            );

        LineRenderer line =
            telegraphObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;

        line.loop = true;

        line.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        line.startColor =
            pattern.AreaTelegraphColor;

        line.endColor =
            pattern.AreaTelegraphColor;

        line.startWidth =
            pattern.AreaTelegraphWidth;

        line.endWidth =
            pattern.AreaTelegraphWidth;

        line.sortingLayerName =
            "Default";

        line.sortingOrder = 10;

        int segments = 64;

        line.positionCount =
            segments;

        Vector3 center =
            transform.position;

        float radius =
            pattern.AxeRadius;

        for (
            int i = 0;
            i < segments;
            i++
        )
        {
            float angle =
                (
                    (float)i /
                    segments
                ) *
                Mathf.PI *
                2f;

            float x =
                Mathf.Cos(angle) *
                radius;

            float y =
                Mathf.Sin(angle) *
                radius;

            line.SetPosition(
                i,
                center +
                new Vector3(
                    x,
                    y,
                    0f
                )
            );
        }

        yield return new WaitForSeconds(
            pattern.AreaTelegraphDuration
        );

        Destroy(telegraphObject);
    }

    // =========================================================
    // PALADIN SWORD RECTANGLE
    // =========================================================

    private IEnumerator RunSwordRectangle(
        BulletPatternData pattern)
    {
        if (pattern.AreaAttackPrefab == null)
        {
            Debug.LogWarning(
                "Sword Rectangle pattern is missing its Area Attack Prefab."
            );

            yield break;
        }

        Vector3 attackPosition =
            transform.position;

        Quaternion attackRotation =
            transform.rotation;

        yield return StartCoroutine(
            ShowRectangleTelegraph(pattern)
        );

        HeroAttackHazard attack =
            Instantiate(
                pattern.AreaAttackPrefab,
                attackPosition,
                attackRotation
            );

        attack.InitializeRectangle(
            pattern.SwordWidth,
            pattern.SwordLength,
            pattern.SwordForwardOffset,
            pattern.AreaAttackDuration,
            pattern.Damage
        );
    }

    // =========================================================
    // PALADIN RECTANGLE TELEGRAPH
    // =========================================================

    private IEnumerator ShowRectangleTelegraph(
        BulletPatternData pattern)
    {
        GameObject telegraphObject =
            new GameObject(
                "Paladin Sword Telegraph"
            );

        LineRenderer line =
            telegraphObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;

        line.loop = true;

        line.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        line.startColor =
            pattern.AreaTelegraphColor;

        line.endColor =
            pattern.AreaTelegraphColor;

        line.startWidth =
            pattern.AreaTelegraphWidth;

        line.endWidth =
            pattern.AreaTelegraphWidth;

        line.sortingLayerName =
            "Default";

        line.sortingOrder = 10;

        line.positionCount = 4;

        Vector3 attackPosition =
            transform.position;

        Vector3 forward =
            transform.up;

        Vector3 right =
            transform.right;

        Vector3 center =
            attackPosition +
            (
                forward *
                pattern.SwordForwardOffset
            );

        float halfWidth =
            pattern.SwordWidth / 2f;

        float halfLength =
            pattern.SwordLength / 2f;

        Vector3 topLeft =
            center +
            (
                right *
                -halfWidth
            ) +
            (
                forward *
                halfLength
            );

        Vector3 topRight =
            center +
            (
                right *
                halfWidth
            ) +
            (
                forward *
                halfLength
            );

        Vector3 bottomRight =
            center +
            (
                right *
                halfWidth
            ) -
            (
                forward *
                halfLength
            );

        Vector3 bottomLeft =
            center +
            (
                right *
                -halfWidth
            ) -
            (
                forward *
                halfLength
            );

        line.SetPosition(
            0,
            topLeft
        );

        line.SetPosition(
            1,
            topRight
        );

        line.SetPosition(
            2,
            bottomRight
        );

        line.SetPosition(
            3,
            bottomLeft
        );

        yield return new WaitForSeconds(
            pattern.AreaTelegraphDuration
        );

        Destroy(telegraphObject);
    }

    // =========================================================
    // SPAWN NORMAL PROJECTILE
    // =========================================================

    private void SpawnProjectile(
        Vector2 direction,
        float speed,
        int damage)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "ProjectileSpawner is missing its Projectile Prefab."
            );

            return;
        }

        HazardProjectile projectile =
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );

        projectile.Initialize(
            direction,
            speed,
            damage
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDisable()
    {
        if (aimedTelegraphLine != null)
        {
            aimedTelegraphLine.enabled = false;
        }
    }
}