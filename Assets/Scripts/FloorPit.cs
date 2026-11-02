using UnityEngine;

public class FloorPit : MonoBehaviour
{
    [SerializeField] private int fallDamage = 1; // Default value just in case

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(fallDamage);
            }
        }
    }
}
