using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 0.75f;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 dodgeDirection;

    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;

    public bool IsDodging => isDodging;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadInput();

        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

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

    private void TryDodge()
    {
        if (isDodging || dodgeCooldownTimer > 0f)
            return;

        dodgeDirection = moveInput;

        // If the player isn't moving, dodge toward the mouse.
        if (dodgeDirection == Vector2.zero)
        {
            Vector3 mouseWorld =
                Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            dodgeDirection = ((Vector2)mouseWorld - rb.position).normalized;
        }

        if (dodgeDirection == Vector2.zero)
            dodgeDirection = Vector2.right;

        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
    }
}

