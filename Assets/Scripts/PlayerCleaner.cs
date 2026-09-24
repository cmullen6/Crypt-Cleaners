using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCleaner : MonoBehaviour
{
    [Header("Active Tool")]
    [SerializeField] private CleaningTool activeTool;

    [Header("Movement Penalty")]
    [SerializeField] private float cleaningMoveMultiplier = 0.35f;

    [Header("Performance Optimization")]
    [SerializeField] private float cleanInterval = 0.05f; // Runs ~20 times per second instead of every frame

    private bool isCleaning;
    private float cleanTimer;

    public bool IsCleaning => isCleaning;

    private void Update()
    {
        if (Mouse.current == null || activeTool == null)
            return;

        isCleaning = Mouse.current.leftButton.isPressed;

        if (!isCleaning)
        {
            cleanTimer = 0f;
            return;
        }

        // Throttle cleaning execution to avoid per-frame physics & tilemap lag
        cleanTimer += Time.deltaTime;
        if (cleanTimer < cleanInterval)
            return;

        cleanTimer = 0f; // Reset timer

        // 1. Get the tool's valid cleaning position in world space
        Vector3 cleanOrigin = activeTool.GetCleanOrigin(transform.position);

        // 2. Perform the overlap sweep using the active tool's radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(cleanOrigin, activeTool.CleaningRadius);

        // 3. Process every object caught in the sweep
        foreach (Collider2D hit in hits)
        {
            // Case A: Individual cleaning spot
            CleaningSpot spot = hit.GetComponent<CleaningSpot>();
            if (spot != null)
            {
                // Scale cleaning rate by cleanInterval since it runs on a tick timer
                spot.Clean(activeTool.CleaningSpeed * cleanInterval);
            }

            // Case B: Destructible tilemap
            DestructableTiles tiles = hit.GetComponent<DestructableTiles>();
            if (tiles != null)
            {
                tiles.EraseTilesAt(cleanOrigin, activeTool.CleaningRadius);
            }
        }
    }

    public float GetMovementMultiplier()
    {
        return isCleaning ? cleaningMoveMultiplier : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (activeTool != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 cleanOrigin = activeTool.GetCleanOrigin(transform.position);
            Gizmos.DrawWireSphere(cleanOrigin, activeTool.CleaningRadius);
        }
    }
}


