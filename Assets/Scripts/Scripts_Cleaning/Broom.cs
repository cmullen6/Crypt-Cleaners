using System.Collections;
using UnityEngine;

public class Broom : CleaningTool
{
    [Header("Broom Burst Settings")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstRange = 2.5f;
    [SerializeField] private float timeBetweenBursts = 0.08f;

    private bool isBursting;

    public override void ExecuteClean(Vector3 playerPos, float interval)
    {
        Vector3 origin = transform.position;
        float startY = origin.y - (burstRange / 2f);

        if (!isBursting)
        {
            StartCoroutine(BurstRoutine(interval));
        }
    }

    private IEnumerator BurstRoutine(float interval)
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            PerformCircleSweep(transform.position, cleaningRadius, interval);
            yield return new WaitForSeconds(timeBetweenBursts);
        }

        isBursting = false;
    }
}
