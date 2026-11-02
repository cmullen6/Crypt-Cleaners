using System.Collections;
using UnityEngine;

public class LaserHazard : MonoBehaviour
{
    [Header("Visual & Collider References")]
    [SerializeField] private LineRenderer telegraphLine;
    [SerializeField] private SpriteRenderer laserSprite;
    [SerializeField] private BoxCollider2D laserCollider;

    [Header("Timing Settings")]
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float activeDuration = 2f;

    [Header("Room Coverage")]
    [SerializeField] private float roomSpanDistance = 50f;

    private float hazardDamage = 15f;

    public void InitializeLaser(LinearPatternData data)
    {
        if (data != null)
        {
            hazardDamage = data.Damage;
        }
    }

    private void Start()
    {
        StartCoroutine(HazardSequence());
    }

    private IEnumerator HazardSequence()
    {
        // 1. Setup the scaling for both telegraph line and beam
        StretchLaserToFitRoom();

        // Phase 1: Telegraph Line ON, Active Beam & Collider OFF
        if (telegraphLine != null) telegraphLine.enabled = true;
        if (laserSprite != null) laserSprite.enabled = false;
        if (laserCollider != null) laserCollider.enabled = false;

        yield return new WaitForSeconds(warningDuration);

        // Phase 2: Telegraph Line OFF, Active Beam & Collider ON
        if (telegraphLine != null) telegraphLine.enabled = false;
        if (laserSprite != null) laserSprite.enabled = true;
        if (laserCollider != null) laserCollider.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        // Cleanup: Destroy the hazard when finished
        Destroy(gameObject);
    }

    private void StretchLaserToFitRoom()
    {
        // Configure Line Renderer (Telegraph) endpoints
        if (telegraphLine != null)
        {
            telegraphLine.useWorldSpace = false;
            telegraphLine.positionCount = 2;
            telegraphLine.SetPosition(0, Vector3.zero);
            telegraphLine.SetPosition(1, Vector3.left * roomSpanDistance); // Or Vector3.right depending on your direction
        }

        // Scale the Sprite Renderer horizontally along X
        if (laserSprite != null)
        {
            Transform spriteTransform = laserSprite.transform;
            spriteTransform.localScale = new Vector3(roomSpanDistance, spriteTransform.localScale.y, 1f);

            // Offset half the distance along X so it fires outward from the origin
            // Use -roomSpanDistance / 2f if firing left, or positive if firing right
            spriteTransform.localPosition = new Vector3(-roomSpanDistance / 2f, 0f, 0f);
        }

        // Configure Box Collider dimensions correctly on their proper axes
        if (laserCollider != null && laserSprite != null)
        {
            Transform spriteTransform = laserSprite.transform;

            // Define your desired visual thickness
            float desiredWidth = 1f;

            // Calculate world-space compensated dimensions
            float exactWidth = desiredWidth / Mathf.Max(0.001f, Mathf.Abs(spriteTransform.localScale.x));
            float exactHeight = roomSpanDistance / Mathf.Max(0.001f, Mathf.Abs(spriteTransform.localScale.y));

            // Width goes to X, Height goes to Y
            laserCollider.size = new Vector2(exactWidth, exactHeight);
            laserCollider.offset = new Vector2(0f, 0f);
        }
    }
}