using UnityEngine;

[CreateAssetMenu(
    fileName = "New Bullet Pattern",
    menuName = "Crypt Cleaners/Bullet Pattern"
)]
public class BulletPatternData : ScriptableObject
{
    public enum BulletPatternType
    {
        RadialBurst,
        Spiral,
        AimedBurst,
        AxeCircle,
        SwordRectangle
    }

    [Header("Pattern")]
    [SerializeField] private BulletPatternType patternType;

    [Header("Timing")]
    [Tooltip("Used by patterns that fire continuously, such as Spiral.")]
    [SerializeField] private float duration = 3f;

    [Tooltip("Time to wait after this pattern finishes.")]
    [SerializeField] private float delayAfterPattern = 1f;

    [Header("Projectile")]
    [SerializeField] private int projectileCount = 8;

    [SerializeField] private float projectileSpeed = 4f;

    [SerializeField] private int damage = 1;

    [Header("Burst Settings")]
    [SerializeField] private int burstCount = 1;

    [SerializeField] private float fireInterval = 0.5f;

    [Header("Spiral Settings")]
    [Tooltip("How many degrees the spiral rotates after each projectile.")]
    [SerializeField] private float spiralRotation = 30f;

    [Header("Aimed Attack Telegraph")]
    [Tooltip("How long the aimed attack warning is visible.")]
    [SerializeField] private float telegraphDuration = 1.25f;

    [Tooltip("Width of the aimed attack warning line.")]
    [SerializeField] private float telegraphWidth = 0.08f;

    [Tooltip("How far the aimed attack warning line extends.")]
    [SerializeField] private float telegraphLength = 20f;

    [Header("Area Attack")]
    [Tooltip("Prefab used by the Barbarian and Paladin attacks.")]
    [SerializeField] private HeroAttackHazard areaAttackPrefab;

    [Tooltip("How long the damaging area remains active.")]
    [SerializeField] private float areaAttackDuration = 0.35f;

    [Header("Barbarian Axe Circle")]
    [Tooltip("Radius of the Barbarian's axe attack.")]
    [SerializeField] private float axeRadius = 2f;

    [Header("Paladin Sword Rectangle")]
    [Tooltip("Width of the Paladin's sword attack.")]
    [SerializeField] private float swordWidth = 2f;

    [Tooltip("Length of the Paladin's sword attack.")]
    [SerializeField] private float swordLength = 4f;

    [Tooltip("Moves the center of the rectangle forward from the AttackPoint.")]
    [SerializeField] private float swordForwardOffset = 2f;

    [Header("Area Attack Telegraph")]
    [Tooltip("How long the Barbarian or Paladin warning is visible.")]
    [SerializeField] private float areaTelegraphDuration = 1f;

    [Tooltip("Width of the area attack warning.")]
    [SerializeField] private float areaTelegraphWidth = 0.08f;

    [Tooltip("Color of the area attack warning.")]
    [SerializeField] private Color areaTelegraphColor = Color.red;

    // ---------------------------------------------------------
    // PUBLIC READ-ONLY PROPERTIES
    // ---------------------------------------------------------

    public BulletPatternType PatternType
    {
        get { return patternType; }
    }

    public float Duration
    {
        get { return duration; }
    }

    public float DelayAfterPattern
    {
        get { return delayAfterPattern; }
    }

    public int ProjectileCount
    {
        get { return projectileCount; }
    }

    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    public int Damage
    {
        get { return damage; }
    }

    public int BurstCount
    {
        get { return burstCount; }
    }

    public float FireInterval
    {
        get { return fireInterval; }
    }

    public float SpiralRotation
    {
        get { return spiralRotation; }
    }

    public float TelegraphDuration
    {
        get { return telegraphDuration; }
    }

    public float TelegraphWidth
    {
        get { return telegraphWidth; }
    }

    public float TelegraphLength
    {
        get { return telegraphLength; }
    }

    public HeroAttackHazard AreaAttackPrefab
    {
        get { return areaAttackPrefab; }
    }

    public float AreaAttackDuration
    {
        get { return areaAttackDuration; }
    }

    public float AxeRadius
    {
        get { return axeRadius; }
    }

    public float SwordWidth
    {
        get { return swordWidth; }
    }

    public float SwordLength
    {
        get { return swordLength; }
    }

    public float SwordForwardOffset
    {
        get { return swordForwardOffset; }
    }

    public float AreaTelegraphDuration
    {
        get { return areaTelegraphDuration; }
    }

    public float AreaTelegraphWidth
    {
        get { return areaTelegraphWidth; }
    }

    public Color AreaTelegraphColor
    {
        get { return areaTelegraphColor; }
    }
}