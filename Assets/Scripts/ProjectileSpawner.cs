using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public enum BulletPatternType
    {
        RadialBurst,
        Spiral,
        AimedBurst
    }

    [System.Serializable]
    public class BulletPattern
    {
        [Header("Pattern")]
        public BulletPatternType patternType;

        [Header("Timing")]
        public float duration = 3f;
        public float delayAfterPattern = 1f;

        [Header("Projectile")]
        public int projectileCount = 8;
        public float projectileSpeed = 4f;
        public int damage = 1;

        [Header("Burst Settings")]
        public int burstCount = 1;
        public float fireInterval = 0.5f;

        [Header("Spiral Settings")]
        public float spiralRotation = 30f;

        [Header("Aimed Attack Telegraph")]
        public float telegraphDuration = 1.25f;
        public float telegraphWidth = 0.08f;
    }

    [Header("Projectile")]
    [SerializeField] private HazardProjectile projectilePrefab;

    [Header("Sequence")]
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private bool loopSequence = true;
    [SerializeField] private List<BulletPattern> patternSequence = new List<BulletPattern>();

    [Header("Aimed Attack Telegraph")]
    [SerializeField] private Color telegraphColor = Color.red;

    private Transform playerTransform;
    private LineRenderer telegraphLine;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }

        CreateTelegraphLine();

        StartCoroutine(RunPatternSequence());
    }

    private void CreateTelegraphLine()
    {
        GameObject telegraphObject = new GameObject("Aimed Attack Telegraph");

        telegraphObject.transform.SetParent(transform);
        telegraphObject.transform.localPosition = Vector3.zero;

        telegraphLine = telegraphObject.AddComponent<LineRenderer>();

        telegraphLine.positionCount = 2;

        telegraphLine.startWidth = 0.08f;
        telegraphLine.endWidth = 0.08f;

        telegraphLine.material = new Material(Shader.Find("Sprites/Default"));

        telegraphLine.startColor = telegraphColor;
        telegraphLine.endColor = telegraphColor;

        telegraphLine.sortingLayerName = "Default";
        telegraphLine.sortingOrder = 10;

        telegraphLine.enabled = false;
    }

    private IEnumerator RunPatternSequence()
    {
        yield return new WaitForSeconds(startDelay);

        do
        {
            for (int i = 0; i < patternSequence.Count; i++)
            {
                BulletPattern pattern = patternSequence[i];

                yield return StartCoroutine(ExecutePattern(pattern));

                if (pattern.delayAfterPattern > 0f)
                {
                    yield return new WaitForSeconds(pattern.delayAfterPattern);
                }
            }
        }
        while (loopSequence);
    }

    private IEnumerator ExecutePattern(BulletPattern pattern)
    {
        switch (pattern.patternType)
        {
            case BulletPatternType.RadialBurst:
                yield return StartCoroutine(RunRadialBurst(pattern));
                break;

            case BulletPatternType.Spiral:
                yield return StartCoroutine(RunSpiral(pattern));
                break;

            case BulletPatternType.AimedBurst:
                yield return StartCoroutine(RunAimedBurst(pattern));
                break;
        }
    }

    private IEnumerator RunRadialBurst(BulletPattern pattern)
    {
        int burstCount = Mathf.Max(1, pattern.burstCount);

        for (int burst = 0; burst < burstCount; burst++)
        {
            SpawnRadialBurst(pattern);

            if (burst < burstCount - 1)
            {
                yield return new WaitForSeconds(pattern.fireInterval);
            }
        }
    }

    private void SpawnRadialBurst(BulletPattern pattern)
    {
        int count = Mathf.Max(1, pattern.projectileCount);

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            SpawnProjectile(
                direction,
                pattern.projectileSpeed,
                pattern.damage
            );
        }
    }

    private IEnumerator RunSpiral(BulletPattern pattern)
    {
        float elapsed = 0f;
        float currentAngle = 0f;

        float fireInterval = Mathf.Max(0.01f, pattern.fireInterval);

        while (elapsed < pattern.duration)
        {
            float angle = currentAngle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            SpawnProjectile(
                direction,
                pattern.projectileSpeed,
                pattern.damage
            );

            currentAngle += pattern.spiralRotation;

            yield return new WaitForSeconds(fireInterval);

            elapsed += fireInterval;
        }
    }

    private IEnumerator RunAimedBurst(BulletPattern pattern)
    {
        int burstCount = Mathf.Max(1, pattern.burstCount);

        for (int i = 0; i < burstCount; i++)
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            if (playerTransform != null)
            {
                Vector2 direction = (
                    playerTransform.position - transform.position
                ).normalized;

                yield return StartCoroutine(
                    ShowAimedTelegraph(
                        direction,
                        pattern.telegraphDuration,
                        pattern.telegraphWidth
                    )
                );

                SpawnProjectile(
                    direction,
                    pattern.projectileSpeed,
                    pattern.damage
                );
            }

            if (i < burstCount - 1)
            {
                yield return new WaitForSeconds(pattern.fireInterval);
            }
        }
    }

    private IEnumerator ShowAimedTelegraph(
        Vector2 direction,
        float duration,
        float width)
    {
        if (telegraphLine == null)
            yield break;

        telegraphLine.enabled = true;

        telegraphLine.startWidth = width;
        telegraphLine.endWidth = width;

        Vector3 startPosition = transform.position;

        Vector3 endPosition =
            startPosition + (Vector3)(direction.normalized * 20f);

        telegraphLine.SetPosition(0, startPosition);
        telegraphLine.SetPosition(1, endPosition);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (playerTransform != null)
            {
                Vector2 updatedDirection = (
                    playerTransform.position - transform.position
                ).normalized;

                endPosition =
                    transform.position +
                    (Vector3)(updatedDirection * 20f);

                telegraphLine.SetPosition(
                    0,
                    transform.position
                );

                telegraphLine.SetPosition(
                    1,
                    endPosition
                );
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        telegraphLine.enabled = false;
    }

    private void SpawnProjectile(
        Vector2 direction,
        float speed,
        int damage)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "ProjectileSpawner is missing its projectile prefab."
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

    private void OnDisable()
    {
        if (telegraphLine != null)
        {
            telegraphLine.enabled = false;
        }
    }
}
