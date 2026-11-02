using UnityEngine;

[CreateAssetMenu(
    fileName = "SpiralPatternData", 
    menuName = "Crypt Cleaners/Bullet Pattern/SpiralPatternData"
)]
public class SpiralPatternData : BulletPatternData
{
    [Header("Spiral Pattern Settings")]
    [SerializeField] private float duration;
    [SerializeField] private float fireInterval;
    [SerializeField] private float spiralRotation;

    [Header("Read-Only Properties")]
    public float Duration => duration;
    public float FireInterval => fireInterval;
    public float SpiralRotation => spiralRotation;
    
}
