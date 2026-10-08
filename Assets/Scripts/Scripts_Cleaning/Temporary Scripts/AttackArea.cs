using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private int damage = 3;


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() != null)
        {
            PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
            health.TakeDamage(damage);
        }
    }
}
