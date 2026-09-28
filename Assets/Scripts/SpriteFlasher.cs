using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class SpriteFlasher : MonoBehaviour
{
    [Header("Flash Materials")]
    [SerializeField] private Material flashMaterial;

    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    
    // Flashes the sprite solid white for a specific duration.
    public void FlashWhite(float duration)
    {
        if (flashMaterial == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        // Swap to white material
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        // Reset to original sprite material
        spriteRenderer.material = defaultMaterial;
        flashCoroutine = null;
    }

    
    // Instantly restores default material if dodge gets canceled.
    public void ResetMaterial()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        spriteRenderer.material = defaultMaterial;
    }
}
