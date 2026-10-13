using UnityEngine;
using UnityEngine.Tilemaps;

public class GooHealth : MonoBehaviour
{
    public int maxHealth = 24;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
       
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
