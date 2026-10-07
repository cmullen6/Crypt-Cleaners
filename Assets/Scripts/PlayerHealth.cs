using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 15;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private int resurrectMe = 3;
    [SerializeField] private float zoomTimer = 3f;
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

    private void Update()
    {

        // Counts down the timer until the lose panel shows up
        // This only starts on death
        // Sets back to zero just to make sure the timer stops, and all numbers go back to how they should be
        if (currentHealth <= 0)
        {

            zoomTimer -= Time.unscaledDeltaTime;

            if (zoomTimer <= 0f)
            {

                zoomTimer = 0f;

            }

        }

        // Turns on lose panel once zoom is over & updates amount of resurrects left
        if (zoomTimer <= 0f)
        {

            resurrectText.text = "resurrections Left: " + resurrectMe;

            losePanel.SetActive(true);

        }

    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // If player dies, this starts the lose zoom in
        if (currentHealth <= 1)
        {

           Time.timeScale = 0f;

            // Zooms in on player to show death animation
            ZoomCameraIn();
         

        }

        // Checks the independent iFrames timer on PlayerController
        if (playerController != null && playerController.IsInvincible)
            return;

        currentHealth -= damage;

        
    }

    // This respawns the player, only allowed 3
    public void Resurrect()
    {

        resurrectMe -= 1;

        // If respawnable, this will zoom out the camera and respawn the player
        if (resurrectMe >= 0)
        {

            Time.timeScale = 1f;

            Die();

            ZoomCameraOut();

        }
        else
        {

            return;

        }

    }

    // Zooms the camera in on the player on death
    private void ZoomCameraIn()
    {

        camera.orthographicSize = 3;

    }

    // Zooms the camera back out
    private void ZoomCameraOut()
    {

        camera.orthographicSize = 10;

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

        losePanel.SetActive(false);
        zoomTimer = 3f;

        transform.position = startingPosition;

        currentHealth = maxHealth;
        isDead = false;
    }
}
