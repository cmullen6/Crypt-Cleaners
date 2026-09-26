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
    public CleaningTool ActiveTool => activeTool;

    private void Update()
    {
        // Safety check for active input and assigned tool
        if (Mouse.current == null || activeTool == null)
            return;

        // Track left mouse click / hold
        isCleaning = Mouse.current.leftButton.isPressed;

        if (!isCleaning)
        {
            cleanTimer = 0f;
            return;
        }

        // Throttle execution to avoid per-frame physics & tilemap mesh rebuilds
        cleanTimer += Time.deltaTime;
        if (cleanTimer < cleanInterval)
            return;

        cleanTimer = 0f; // Reset timer

        // Trigger the active tool's unique spatial cleaning pattern (Broom, Mop, or Sponge)
        activeTool.ExecuteClean(transform.position, cleanInterval);
    }

    
    public float GetMovementMultiplier()
    {
        return isCleaning ? cleaningMoveMultiplier : 1f;
    }

    
    // Allows runtime tool switching
    public void SetActiveTool(CleaningTool newTool)
    {
        if (newTool != null)
        {
            activeTool = newTool;
        }
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


