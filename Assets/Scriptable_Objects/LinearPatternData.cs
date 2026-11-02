using UnityEngine;

[CreateAssetMenu(
    fileName = "LinearPatternData", 
    menuName = "Crypt Cleaners/Bullet Pattern/LinearPatternData"
)]
public class LinearPatternData : BulletPatternData
{
    [Header("Linear Pattern Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifetime;
    [SerializeField] private float lingeringDuration;
    [SerializeField] private float projectileScale;
    [SerializeField] private bool isPiercing;
    [SerializeField] private bool isReturning;

    [Header("Quickdraw Burst Settings")]
    public bool isBurst = false;
    public int burstCount = 3;          // Number of arrows in the burst (e.g., 3 or 4)
    public float burstInterval = 0.1f;  // Delay between each arrow in seconds

    [Header("Read-Only Properties")]
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public float LingeringDuration => lingeringDuration;
    public float ProjectileScale => projectileScale;
    public bool IsPiercing => isPiercing;
    public bool IsReturning => isReturning;

}
