using System.Collections;
using UnityEngine;

public class LaserHazard : MonoBehaviour
{
    [Header("Visual & Collider References")]
    [SerializeField] private LineRenderer telegraphLine;
    [SerializeField] private SpriteRenderer laserSprite;
    private CapsuleCollider2D laserCollider;

    [Header("Timing Settings")]
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float activeDuration = 2f;

    [Header("Room Coverage")]
    [SerializeField] private float roomSpanDistance = 50f;

    private float hazardDamage = 2f;
    private bool isHazardActive = false;


    public void InitializeLaser(LinearPatternData data)
    {
        if (data != null)
        {
            hazardDamage = data.Damage;
        }

        StretchLaserToFitRoom();
    }

    private void Start()
    {
        StartCoroutine(HazardSequence());
    }

    private IEnumerator HazardSequence()
    {
        // Setup the scaling for both telegraph line and beam
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
        isHazardActive = true;  // Turn damage ON for the active beam phase

        yield return new WaitForSeconds(activeDuration);

        // Cleanup: Destroy the hazard when finished
        isHazardActive = false; // Turn damage OFF
        Destroy(transform.root.gameObject);
    }

    private void StretchLaserToFitRoom()
    {
        // Automatically fetch and cache the Capsule Collider 2D reference
        if (laserCollider == null)
        {
            laserCollider = GetComponent<CapsuleCollider2D>();
        }

        // Configure Line Renderer (Telegraph)
        if (telegraphLine != null)
        {
            telegraphLine.useWorldSpace = false;
            telegraphLine.positionCount = 2;
            telegraphLine.SetPosition(0, Vector3.zero);
            telegraphLine.SetPosition(1, Vector3.down * roomSpanDistance);
        }

        // Scale the Sprite Renderer vertically along room span
        if (laserSprite != null)
        {
            Transform spriteTransform = laserSprite.transform;
            spriteTransform.localScale = new Vector3(spriteTransform.localScale.x, roomSpanDistance, 1f);
            spriteTransform.localPosition = new Vector3(0f, -roomSpanDistance / 2f, 0f);
        }

        // Size and offset the Capsule Collider 2D
        if (laserCollider != null && laserSprite != null && laserSprite.sprite != null)
        {
            // Get the base unscaled sprite size (Unity's transform scale will automatically scale the collider to match)
            Vector2 spriteSize = laserSprite.sprite.rect.size / laserSprite.sprite.pixelsPerUnit;

            laserCollider.size = spriteSize;
            laserCollider.offset = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions if the laser is still in its warning phase
        if (!isHazardActive) return;

        // Check if the collider belongs to the player
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(Mathf.RoundToInt(hazardDamage));
        }
    }
}