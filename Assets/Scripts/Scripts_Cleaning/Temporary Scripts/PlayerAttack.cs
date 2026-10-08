using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea = default;

    private bool isAttacking = false;

    [SerializeField] private float timeToAttack = 0.25f;

    [SerializeField] private float timer = 0f;



    void Start()
    {
        attackArea = transform.GetChild(5).gameObject;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (isAttacking)
        {
            timer += Time.deltaTime;
            
            if (timer >= timeToAttack)
            {
                timer = 0f;
                isAttacking = false;
                attackArea.SetActive(isAttacking);
            }
        }
    }

    
    private void Attack()
    {
        isAttacking = true;
        attackArea.SetActive(isAttacking);
    }
}
