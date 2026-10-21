
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Aiming")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform crosshair;
    [SerializeField] private float aimRadius = 5.5f;
    [SerializeField] private bool limitAimRadius = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteFlasher spriteFlasher;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float iFramesDuration = 1.5f;
    [SerializeField] private float dodgeCooldown = 0.75f;
    [SerializeField] private Image cooldownImage;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 aimDirection = Vector2.right;
    private Vector2 dodgeDirection;

    private float dodgeTimer;
    private float iFramesTimer;
    private float dodgeCooldownTimer;

    private bool isDodging;
    private bool cooldownRoutineRunning;

    public bool IsDodging => isDodging;
    public bool IsInvincible => iFramesTimer > 0f;
    public Vector2 FacingDirection => aimDirection;
    public Vector2 MoveInput => moveInput;
    public Transform CrosshairTransform => crosshair;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteFlasher == null)
            spriteFlasher = GetComponent<SpriteFlasher>();
    }

    private void Start()
    {
        Cursor.visible = false;

        if (cooldownImage != null)
            cooldownImage.fillAmount = 1f;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        ReadMovementInput();
        UpdateAim();

        if (iFramesTimer > 0f)
            iFramesTimer -= Time.deltaTime;

        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0f)
                isDodging = false;
        }

        UpdateAnimation();
        UpdateCooldownUI();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDodging)
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
        else
            rb.linearVelocity = moveInput * moveSpeed;
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            TryDodge();
    }

    private void UpdateAim()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z -
                          transform.position.z)
            )
        );

        Vector2 offset = (Vector2)mouseWorldPosition -
                         (Vector2)transform.position;

        if (limitAimRadius)
            offset = Vector2.ClampMagnitude(offset, aimRadius);

        if (offset.sqrMagnitude > 0.001f)
            aimDirection = offset.normalized;

        if (crosshair != null)
        {
            crosshair.position = new Vector3(
                transform.position.x + offset.x,
                transform.position.y + offset.y,
                crosshair.position.z
            );
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        // Aim controls facing, not movement.
        Vector2 direction = GetEightDirection(aimDirection);

        animator.SetFloat("FaceX", direction.x);
        animator.SetFloat("FaceY", direction.y);
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0.01f);
        animator.SetBool("IsDodging", isDodging);
    }

    private Vector2 GetEightDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return Vector2.right;

        float angle = Mathf.Atan2(direction.y, direction.x)
                      * Mathf.Rad2Deg;

        int sector = Mathf.RoundToInt(angle / 45f);
        float snappedAngle = sector * 45f * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(snappedAngle),
            Mathf.Sin(snappedAngle)
        ).normalized;
    }

    private void TryDodge()
    {
        if (isDodging || dodgeCooldownTimer > 0f)
            return;

        dodgeDirection = moveInput.sqrMagnitude > 0.01f
            ? moveInput.normalized
            : aimDirection;

        isDodging = true;
        dodgeTimer = dodgeDuration;
        iFramesTimer = iFramesDuration;
        dodgeCooldownTimer = dodgeCooldown;

        if (spriteFlasher != null)
            spriteFlasher.FlashWhite(iFramesDuration);
    }

    private void UpdateCooldownUI()
    {
        if (cooldownImage == null)
            return;

        cooldownImage.fillAmount = dodgeCooldown <= 0f
            ? 1f
            : 1f - Mathf.Clamp01(dodgeCooldownTimer / dodgeCooldown);
    }
}
