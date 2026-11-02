using UnityEngine;

[CreateAssetMenu(
    fileName = "MeleeAreaPatternData", 
    menuName = "Crypt Cleaners/Bullet Pattern/MeleeAreaPatternData"
)]
public class MeleeAreaPatternData : BulletPatternData
{
    public enum MeleeShape
    {
        Circle,
        Rectangle
    }

    [Header("Shape Configuration")]
    [SerializeField] private MeleeShape attackShape = MeleeShape.Circle;

    [Header("Melee Area Pattern Settings")]
    [SerializeField] private HeroAttackHazard areaAttackPrefab;
    [SerializeField] private float areaAttackDuration;
    [SerializeField] private float areaTelegraphDuration;
    [SerializeField] private float areaTelegraphWidth;
    [SerializeField] private Color areaTelegraphColor;
    [SerializeField] private float shapeSizeRadiusOrWidth;
    [SerializeField] private float shapeSizeLengthOrOffset;
    [SerializeField] private int strikeCount;

    [Header("Read-Only Properties")]
    public MeleeShape AttackShape => attackShape;
    public HeroAttackHazard AreaAttackPrefab => areaAttackPrefab;
    public float AreaAttackDuration => areaAttackDuration;
    public float AreaTelegraphDuration => areaTelegraphDuration;
    public float AreaTelegraphWidth => areaTelegraphWidth;
    public Color AreaTelegraphColor => areaTelegraphColor;
    public float ShapeSizeRadiusOrWidth => shapeSizeRadiusOrWidth;
    public float ShapeSizeLengthOrOffset => shapeSizeLengthOrOffset;
    public int StrikeCount => strikeCount;
}
