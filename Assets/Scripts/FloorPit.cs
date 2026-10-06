using UnityEngine;

public class FloorPit : MonoBehaviour
{
    [SerializeField] int fallDamage;
    [SerializeField] PlayerHealth playerHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(fallDamage);
        }
    }
}
