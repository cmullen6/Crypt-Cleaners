using UnityEngine;

public class Sponge : CleaningTool
{
    [Header("Sponge Settings")]
    [SerializeField] private Vector3 closeOffset = new Vector3(0f, 0.4f, 0f);

    public override void ExecuteClean(Vector3 playerPos, float interval)
    {
        Vector3 closeOrigin = playerPos + closeOffset;
        PerformCircleSweep(closeOrigin, cleaningRadius, interval);
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + closeOffset, cleaningRadius);
    }
}