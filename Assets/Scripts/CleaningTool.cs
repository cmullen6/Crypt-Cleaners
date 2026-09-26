using UnityEngine;

public class CleaningTool : MonoBehaviour
{
    [Header("Tool Reach & Area")]
    [SerializeField] protected float cleaningRadius = 1.2f;
    [SerializeField] protected float cleaningSpeed = 30f;

    public float CleaningRadius => cleaningRadius;
    public float CleaningSpeed => cleaningSpeed;

    public virtual Vector3 GetCleanOrigin(Vector3 playerPosition)
    {
        return transform.position;
    }

    public virtual void ExecuteClean(Vector3 playerPos, float interval)
    {
        PerformCircleSweep(transform.position, cleaningRadius, interval);
    }

    protected void PerformCircleSweep(Vector3 origin, float radius, float interval)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

        foreach (Collider2D hit in hits)
        {
            CleaningSpot spot = hit.GetComponent<CleaningSpot>();
            if (spot != null)
            {
                spot.Clean(cleaningSpeed * interval);
            }

            DestructableTiles tiles = hit.GetComponent<DestructableTiles>();
            if (tiles != null)
            {
                tiles.EraseTilesAt(origin, radius);
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cleaningRadius);
    }
}

