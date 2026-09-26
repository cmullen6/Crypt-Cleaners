using UnityEngine;

public class Mop : CleaningTool
{
    [Header("Mop Arc Settings")]
    [SerializeField] private float arcWidth = 2.5f;
    [SerializeField] private int sweepPoints = 5;

    public override void ExecuteClean(Vector3 playerPos, float interval)
    {
        Vector3 origin = transform.position;
        float startX = origin.x - (arcWidth / 2f);
        float step = arcWidth / (sweepPoints - 1);

        // Sweep multiple overlapping circles across the horizontal line
        for (int i = 0; i < sweepPoints; i++)
        {
            Vector3 point = new Vector3(startX + (step * i), origin.y, origin.z);
            PerformCircleSweep(point, cleaningRadius, interval);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Gizmos.DrawLine(
            new Vector3(origin.x - arcWidth / 2f, origin.y, origin.z),
            new Vector3(origin.x + arcWidth / 2f, origin.y, origin.z)
        );
    }
}
