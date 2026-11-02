using UnityEngine;

[CreateAssetMenu(
    fileName = "RadialBurstPatternData", 
    menuName = "Crypt Cleaners/Bullet Pattern/RadialBurstPatternData"
)]
public class RadialBurstPatternData : BulletPatternData
{
    [Header("Radial Burst Pattern Settings")]
    [SerializeField] private int projectileCount;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private int burstCount;
    [SerializeField] private float fireInterval;

    [Header("Read-Only Properties")]
    public int ProjectileCount => projectileCount;
    public float ProjectileSpeed => projectileSpeed;
    public int BurstCount => burstCount;
    public float FireInterval => fireInterval;

}
