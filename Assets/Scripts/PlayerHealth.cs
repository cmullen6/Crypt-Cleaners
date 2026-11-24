using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private int resurrectMe = 3;
    [SerializeField] private TextMeshProUGUI resurrectText;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject trash;
    [SerializeField] private Button respawnButton;

    [Header("Hitstop")]
    [SerializeField] private float hurtHitstop = 0.08f;

    private int currentHealth;
    private bool isDead;
    private bool deathSequenceStarted;

    private Vector3 startingPosition;
    private PlayerController playerController;
    private Rigidbody2D rb;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        startingPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        Cursor.visible = false;
    }

    private void Update()
    {
        if (!deathSequenceStarted)
            return;

        
        if (currentHealth <= 0f)
        {

            if (resurrectMe <= 0f)
                respawnButton.interactable = false;

            if (resurrectText != null)
                resurrectText.text = "Resurrections Left: " + resurrectMe;

            if (losePanel != null)
                losePanel.SetActive(true);

            Cursor.visible = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        if (playerController != null && playerController.IsInvincible)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            BeginDeathSequence();
            return;
        }

        if (playerController != null)
            playerController.TriggerDamageInvulnerability();

        if (HitStopManager.Instance != null)
            HitStopManager.Instance.StopTime(hurtHitstop);
    }

    private void BeginDeathSequence()
    {
        if (deathSequenceStarted)
            return;

        deathSequenceStarted = true;
        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Death pause is managed separately from hitstop.
        Time.timeScale = 0f;
    }

    public void Resurrect()
    {
        if (!deathSequenceStarted || resurrectMe <= 0)
            return;

        respawnButton.interactable = false;

        resurrectMe--;

        Time.timeScale = 1f;

        if (losePanel != null)
            losePanel.SetActive(false);

        Cursor.visible = false;

        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        SpawnTrash();
        CancelInvoke(nameof(Respawn));
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        transform.position = startingPosition;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        currentHealth = maxHealth;
        isDead = false;
        deathSequenceStarted = false;
        respawnButton.interactable = true;

        if (losePanel != null)
            losePanel.SetActive(false);

        Cursor.visible = false;
    }

    private void SpawnTrash()
    {
        if (trash != null)
            Instantiate(trash, transform.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Respawn));
    }
}
