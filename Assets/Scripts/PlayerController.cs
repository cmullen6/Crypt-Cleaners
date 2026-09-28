using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float iFramesDuration = 1.5f;
    [SerializeField] private float dodgeCooldown = 0.75f;

    [Header("Tool Pivot")]
    [SerializeField] private Transform toolHolder; // Drag ToolHolder child GameObject here

    private Rigidbody2D rb;
    private SpriteFlasher spriteFlasher;

    private Vector2 moveInput;
    private Vector2 dodgeDirection;
    private Vector2 lastFacingDirection = Vector2.right; // Default facing right

    private bool isDodging;
    private float dodgeTimer;
    private float iFramesTimer;
    private float dodgeCooldownTimer;

    public bool IsDodging => isDodging;
    public bool IsInvincible => iFramesTimer > 0f;
    public Vector2 FacingDirection => lastFacingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteFlasher = GetComponent<SpriteFlasher>();
    }

    private void Update()
    {
        ReadInput();
        UpdateFacingAndToolHolder();

        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        if (iFramesTimer > 0f)
            iFramesTimer -= Time.deltaTime;

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDodging)
        {
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        moveInput = moveInput.normalized;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryDodge();
        }
    }

    private void UpdateFacingAndToolHolder()
    {
        // Only update facing direction when the player is providing movement input
        if (moveInput != Vector2.zero)
        {
            lastFacingDirection = moveInput;

            if (toolHolder != null)
            {
                // Calculate angle in degrees from movement vector
                float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

                // Rotate ToolHolder around the player
                toolHolder.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    private void TryDodge()
    {
        if (isDodging || dodgeCooldownTimer > 0f)
            return;

        dodgeDirection = moveInput;

        // If not pressing keys, dodge in the direction the player was last facing
        if (dodgeDirection == Vector2.zero)
        {
            dodgeDirection = lastFacingDirection;
        }

        isDodging = true;
        dodgeTimer = dodgeDuration;
        iFramesTimer = iFramesDuration;
        dodgeCooldownTimer = dodgeCooldown;

        if (spriteFlasher != null)
        {
            spriteFlasher.FlashWhite(iFramesDuration);
        }
    }
}
