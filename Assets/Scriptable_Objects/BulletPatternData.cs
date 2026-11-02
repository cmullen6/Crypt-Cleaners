using UnityEngine;

public abstract class BulletPatternData : ScriptableObject
{
    public enum BulletPatternType
    {
        Linear,
        MeleeArea,
        RadialBurst,
        Spiral,
        Spread
    }

    [Header("Base Pattern Settings")]
    [SerializeField] protected float delayAfterPattern;
    [SerializeField] protected int damage;

    public float DelayAfterPattern => delayAfterPattern;
    public int Damage => damage;

    [Header("Projectile Config")]
    public GameObject ProjectilePrefab;

}