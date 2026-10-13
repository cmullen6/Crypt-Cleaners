using UnityEngine;


public class AttackArea : MonoBehaviour
{
    [SerializeField] private int damage = 3;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<GooHealth>() != null)
        {
            GooHealth health = collider.gameObject.GetComponent<GooHealth>();
            health.TakeDamage(damage);
        }
    }
}
