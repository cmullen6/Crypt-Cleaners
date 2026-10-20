using UnityEngine;


public class AttackArea : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    private bool hasDealtDamage = false;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(hasDealtDamage == true)
        {
            return;
        }

        if (collider.gameObject.GetComponent<GooHealth>() != null)
        {
            GooHealth health = collider.gameObject.GetComponent<GooHealth>();
            health.TakeDamage(damage);
            hasDealtDamage = true;
        }

  

    }

    private void OnEnable()
    {
        hasDealtDamage = false;
    }
}
