using UnityEngine;

[CreateAssetMenu(
    fileName = "SpreadPatternData",
    menuName = "Crypt Cleaners/Bullet Pattern/SpreadPatternData"
)]
public class SpreadPatternData : BulletPatternData
{
    [Header("Spread Pattern Settings")]
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float spreadAngle = 45f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileScale = 1f;
    [SerializeField] private bool isPiercing = false;

    [Header("Read-Only Properties")]
    public int ProjectileCount => projectileCount;
    public float SpreadAngle => spreadAngle;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileScale => projectileScale;
    public bool IsPiercing => isPiercing;
}
