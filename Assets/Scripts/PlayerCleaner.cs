using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCleaner : MonoBehaviour
{
    [Header("Cleaning")]
    [SerializeField] private float cleaningRadius = 1.5f;
    [SerializeField] private float cleaningSpeed = 30f;

    [Header("Movement Penalty")]
    [SerializeField] private float cleaningMoveMultiplier = 0.35f;

    private bool isCleaning;

    public bool IsCleaning => isCleaning;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        isCleaning = Mouse.current.leftButton.isPressed;

        if (!isCleaning)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            cleaningRadius
        );

        foreach (Collider2D hit in hits)
        {
            CleaningSpot spot = hit.GetComponent<CleaningSpot>();

            if (spot != null)
            {
                spot.Clean(cleaningSpeed * Time.deltaTime);
            }
        }
    }

    public float GetMovementMultiplier()
    {
        return isCleaning ? cleaningMoveMultiplier : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, cleaningRadius);
    }
}


