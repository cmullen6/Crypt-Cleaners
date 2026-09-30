using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 15;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private int resurrectMe = 3;
    [SerializeField] private float zoomTimer = 5f;
    [SerializeField] private Camera camera;
    [SerializeField] private TextMeshProUGUI resurrectText;
    [SerializeField] private GameObject losePanel;

    private int currentHealth;
    private bool isDead;

    private Vector3 startingPosition;
    private PlayerController playerController;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        startingPosition = transform.position;
        currentHealth = maxHealth;

        // Sets panel to off on start
        losePanel.SetActive(false);

        // Cache reference to avoid GetComponent allocations on hit
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // Checks the independent iFrames timer on PlayerController
        if (playerController != null && playerController.IsInvincible)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {

            // Zooms in on player to show death animation





            zoomTimer -= Time.deltaTime;

            // Turns on lose panel once zoom is over
            if (zoomTimer < 0f)
            {

                resurrectText.text = "resurrections Left: " + resurrectMe;

                losePanel.SetActive(true);

            }

        }
    }

    // This respawns the player, only allowed 3
    public void Resurrect()
    {

        resurrectMe -= 1;
        zoomTimer = 5;

        // If respawnable, this will zoom out the camera and respawn the player
        if (resurrectMe >= 0)
        {





            Die();

        }
        else
        {

            return;

        }


    }

    private void Die()
    {
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        transform.position = startingPosition;

        currentHealth = maxHealth;
        isDead = false;
    }
}
