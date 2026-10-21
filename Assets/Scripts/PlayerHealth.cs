using TMPro;
using UnityEngine;

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
    [SerializeField] private GameObject trash;

    [Header("Hitstop")]
    [SerializeField] private float hurtHitstop = 0.08f;

    [Header("Death Camera")]
    [SerializeField] private float deathZoomSize = 3f;
    [SerializeField] private float normalZoomSize = 10f;

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

        if (camera == null)
            camera = Camera.main;

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

        // Unscaled time continues while the death zoom is paused.
        zoomTimer -= Time.unscaledDeltaTime;

        if (zoomTimer <= 0f)
        {
            zoomTimer = 0f;

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

        if (HitStopManager.Instance != null)
            HitStopManager.Instance.StopTime(hurtHitstop);
    }

    private void BeginDeathSequence()
    {
        if (deathSequenceStarted)
            return;

        deathSequenceStarted = true;
        isDead = true;
        zoomTimer = 3f;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (camera != null)
            camera.orthographicSize = deathZoomSize;

        // Death pause is managed separately from hitstop.
        Time.timeScale = 0f;
    }

    public void Resurrect()
    {
        if (!deathSequenceStarted || resurrectMe <= 0)
            return;

        resurrectMe--;

        Time.timeScale = 1f;

        if (losePanel != null)
            losePanel.SetActive(false);

        if (camera != null)
            camera.orthographicSize = normalZoomSize;

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
        zoomTimer = 3f;

        if (losePanel != null)
            losePanel.SetActive(false);

        if (camera != null)
            camera.orthographicSize = normalZoomSize;

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
