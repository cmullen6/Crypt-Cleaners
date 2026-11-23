
using System.Collections.Generic;
using UnityEngine;

public class CleaningTool : MonoBehaviour
{
    [Header("Mop Cleaning")]
    [SerializeField, Min(0.01f)]
    private float cleaningRadius = 0.25f;

    [SerializeField, Min(0f)]
    private float cleaningSpeed = 30f;

    [Header("Mop Contact Point")]
    [Tooltip("The point at the mop head that contacts the ground.")]
    [SerializeField] private Transform cleaningOrigin;

    [Header("Goo Cleaning")]
    [Tooltip("All GooSurface objects that this tool can clean.")]
    [SerializeField] private GooSurface[] gooSurfaces;

    public float CleaningRadius => cleaningRadius;
    public float CleaningSpeed => cleaningSpeed;

    private Vector3 previousPosition;
    private bool hasPreviousPosition;

    private readonly HashSet<CleaningSpot> cleanedThisSample =
        new HashSet<CleaningSpot>();

    private readonly HashSet<DestructableTiles> tilesThisSample =
        new HashSet<DestructableTiles>();

    private void LateUpdate()
    {
        // Remember the mop's position while it is not cleaning.
        if (!hasPreviousPosition)
        {
            previousPosition = GetOriginPosition();
            hasPreviousPosition = true;
        }
    }

    public void BeginCleaning()
    {
        // Start the cleaning sweep at the current mop position.
        previousPosition = GetOriginPosition();
        hasPreviousPosition = true;
    }

    public virtual Vector3 GetCleanOrigin(Vector3 playerPosition)
    {
        return GetOriginPosition();
    }

    public virtual void ExecuteClean(Vector3 playerPos, float interval)
    {
        if (interval <= 0f)
            return;

        Vector3 currentPosition = GetOriginPosition();

        if (!hasPreviousPosition)
        {
            previousPosition = currentPosition;
            hasPreviousPosition = true;
        }

        Vector2 start = previousPosition;
        Vector2 end = currentPosition;

        float distance = Vector2.Distance(start, end);

        // Smaller steps help prevent fast mop movement
        // from skipping over cleaning targets.
        float stepDistance = Mathf.Max(cleaningRadius * 0.5f, 0.05f);

        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / stepDistance)
        );

        float sampleInterval = interval / steps;

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;

            Vector2 samplePosition = Vector2.Lerp(start, end, t);

            CleanAtPosition(samplePosition, sampleInterval);
        }

        previousPosition = currentPosition;
    }

    private Vector3 GetOriginPosition()
    {
        if (cleaningOrigin != null)
            return cleaningOrigin.position;

        return transform.position;
    }

    private void CleanAtPosition(Vector2 position, float interval)
    {
        // Clean every assigned goo surface.
        if (gooSurfaces != null)
        {
            foreach (GooSurface surface in gooSurfaces)
            {
                if (surface == null)
                    continue;

                surface.CleanAtWorldPosition(position, cleaningRadius);
            }
        }

        // Reset these for each sample so the entire sweep
        // contributes cleaning progress.
        cleanedThisSample.Clear();
        tilesThisSample.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            position,
            cleaningRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            // Check for a CleaningSpot.
            CleaningSpot spot = hit.GetComponent<CleaningSpot>();

            if (spot == null)
                spot = hit.GetComponentInParent<CleaningSpot>();

            if (spot != null && cleanedThisSample.Add(spot))
            {
                spot.Clean(cleaningSpeed * interval);
            }

            // Check for destructible tiles.
            DestructableTiles tiles =
                hit.GetComponent<DestructableTiles>();

            if (tiles == null)
                tiles = hit.GetComponentInParent<DestructableTiles>();

            if (tiles != null && tilesThisSample.Add(tiles))
            {
                tiles.EraseTilesAt(position, cleaningRadius);
            }
        }
    }

    public void StopCleaning()
    {
        hasPreviousPosition = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 position = cleaningOrigin != null
            ? cleaningOrigin.position
            : transform.position;

        Gizmos.DrawWireSphere(position, cleaningRadius);
    }
}
