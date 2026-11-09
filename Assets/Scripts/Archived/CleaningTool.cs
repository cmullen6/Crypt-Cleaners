
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
    [SerializeField] private GooSurface gooSurface;
    public float CleaningRadius => cleaningRadius;
    public float CleaningSpeed => cleaningSpeed;

    private Vector3 previousPosition;
    private bool hasPreviousPosition;

    private readonly HashSet<CleaningSpot> cleanedThisFrame =
        new HashSet<CleaningSpot>();

    private readonly HashSet<DestructableTiles> tilesThisFrame =
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
        // Prevent the mop from sweeping from an old position
        // when the player initially presses the mouse button.
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

        cleanedThisFrame.Clear();
        tilesThisFrame.Clear();

        Vector2 start = previousPosition;
        Vector2 end = currentPosition;

        Vector2 movement = end - start;
        float distance = movement.magnitude;

        // Move in small steps so fast mouse movement
        // cannot skip over cleaning targets.
        float stepDistance = Mathf.Max(cleaningRadius * 0.5f, 0.05f);

        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / stepDistance)
        );

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;

            Vector2 samplePosition = Vector2.Lerp(start, end, t);

            CleanAtPosition(samplePosition, interval / steps);
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
        if (gooSurface != null)
        {
            gooSurface.CleanAtWorldPosition(position, cleaningRadius);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            position,
            cleaningRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            CleaningSpot spot = hit.GetComponent<CleaningSpot>();

            if (spot == null)
                spot = hit.GetComponentInParent<CleaningSpot>();

            if (spot != null && cleanedThisFrame.Add(spot))
            {
                spot.Clean(cleaningSpeed * interval);
            }

            DestructableTiles tiles =
                hit.GetComponent<DestructableTiles>();

            if (tiles == null)
                tiles = hit.GetComponentInParent<DestructableTiles>();

            if (tiles != null && tilesThisFrame.Add(tiles))
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


