using UnityEngine;

public class CleaningTool : MonoBehaviour
{
    [Header("Tool Reach & Area")]
    [SerializeField] private float cleaningRange = 2.5f;   // Max distance reach from player center
    [SerializeField] private float cleaningRadius = 1.2f;  // Size of the cleaning circle
    [SerializeField] private float cleaningSpeed = 30f;   // Cleaning rate per second

    public float CleaningRadius => cleaningRadius;
    public float CleaningSpeed => cleaningSpeed;

    public Vector3 GetCleanOrigin(Vector3 playerPosition)
    {
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        // Yellow: Active cleaning radius around the tool
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cleaningRadius);
    }
}
