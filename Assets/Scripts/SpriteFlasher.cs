using System.Collections;
using UnityEngine;

public class SpriteFlasher : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Header("Flicker Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float flashOpacity = 0.5f; // Alpha opacity of the flash
    [SerializeField] private float flickerInterval = 0.08f; // Speed of blinking in seconds

    private Material originalMaterial;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine flashCoroutine;

    // Property IDs for standard URP shader tint colors
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
            if (targetSpriteRenderer == null)
            {
                targetSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (targetSpriteRenderer != null)
        {
            originalMaterial = targetSpriteRenderer.sharedMaterial;
        }

        propertyBlock = new MaterialPropertyBlock();
    }

    public void FlashWhite(float duration)
    {
        if (flashMaterial == null || targetSpriteRenderer == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlickerRoutine(duration));
    }

    private IEnumerator FlickerRoutine(float duration)
    {
        // Swap to the white flash material
        targetSpriteRenderer.material = flashMaterial;

        float elapsedTime = 0f;
        bool isFlashed = true;

        while (elapsedTime < duration)
        {
            // Toggle alpha between semi-transparent white and low visibility
            float alpha = isFlashed ? flashOpacity : 0.15f;
            Color flashColor = new Color(1f, 1f, 1f, alpha);

            // Apply color tint directly to shader via MaterialPropertyBlock
            targetSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorID, flashColor);
            propertyBlock.SetColor(ColorID, flashColor);
            targetSpriteRenderer.SetPropertyBlock(propertyBlock);

            isFlashed = !isFlashed;

            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        ResetMaterial();
    }

    public void ResetMaterial()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (targetSpriteRenderer != null)
        {
            // Reset property block color and revert to original material
            targetSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.Clear();
            targetSpriteRenderer.SetPropertyBlock(propertyBlock);

            if (originalMaterial != null)
                targetSpriteRenderer.material = originalMaterial;
        }

        flashCoroutine = null;
    }
}